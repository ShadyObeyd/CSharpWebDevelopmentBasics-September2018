namespace SIS.Framework.Routes
{
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using HTTP.Extensions;
    using HTTP.Enums;

    using WebServer.Api.Contracts;
    using WebServer.Results;

    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;

    using Controllers;
    using ActionResults.Contracts;
    using Attributes.Methods;
    using Services.Contracts;
    using Attributes.Action;

    public class ControllerRouter : IHttpHandler
    {
        private const string PropertyNotMappedMessage = "The property {0} could not be mapped";

        private const string ResultTypeNotSupportedMessage = "Type of result is not supported";

        private readonly IDependencyContainer dependencyContainer;

        public ControllerRouter(IDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            string controllerName = string.Empty;
            string actionName = string.Empty;

            if (request.Path == "/" || request.Path == "/favicon.ico")
            {
                controllerName = "Home";
                actionName = "Index";
            }
            else
            {
                string[] requestUrlTokens = request.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);

                controllerName = requestUrlTokens[0].Capitalize();
                actionName = requestUrlTokens[1].Capitalize();
            }

            Controller controller = this.GetController(controllerName);

            if (controller == null)
            {
                throw new ArgumentNullException();
            }

            string requestMethod = request.RequestMethod.ToString();

            var action = this.GetMethod(requestMethod, controller, actionName);

            if (action == null)
            {
                throw new ArgumentNullException();
            }

            object[] actionParameters = this.MapActionParameters(action, request, controller);

            var actionResult = this.InvokeAction(controller, action, actionParameters);

            return this.Authorize(controller, action) ?? this.PrepareResponse(actionResult);
        }

        private IActionResult InvokeAction(Controller controller, MethodInfo action, object[] actionParameters)
        {
            return (IActionResult)action.Invoke(controller, actionParameters);
        }

        private object[] MapActionParameters(MethodInfo action, IHttpRequest request, Controller controller)
        {
            ParameterInfo[] actionParameterInfo = action.GetParameters();

            object[] mappedActionParameters = new object[actionParameterInfo.Length];

            for (int index = 0; index < actionParameterInfo.Length; index++)
            {
                ParameterInfo currentParameterInfo = actionParameterInfo[index];

                if (currentParameterInfo.ParameterType.IsPrimitive || currentParameterInfo.ParameterType == typeof(string))
                {
                    mappedActionParameters[index] = ProcessPrimitiveParameter(currentParameterInfo, request);
                }
                else
                {
                    object bindingModel = this.ProcessesBindingModelParameter(currentParameterInfo, request);
                    controller.ModelState.IsValid = this.IsValid(bindingModel, currentParameterInfo.ParameterType);
                    mappedActionParameters[index] = bindingModel;
                }
            }

            return mappedActionParameters;
        }

        private bool? IsValid(object bindingModel, Type bindingModelType)
        {
            var properties = bindingModelType.GetProperties();

            foreach (var property in properties)
            {
                var propertyValidationAttributes = property
                    .GetCustomAttributes()
                    .Where(ca => ca is ValidationAttribute)
                    .Cast<ValidationAttribute>()
                    .ToList();

                foreach (var validationAttribute in propertyValidationAttributes)
                {
                    var propertyValue = property.GetValue(bindingModel);

                    if (!validationAttribute.IsValid(propertyValue))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private object ProcessesBindingModelParameter(ParameterInfo actionParameter, IHttpRequest request)
        {
            var bindingModelType = actionParameter.ParameterType;

            var bindingModelInstance = Activator.CreateInstance(bindingModelType);

            var bindingModelProperties = bindingModelType.GetProperties();

            foreach (var bindingModelProperty in bindingModelProperties)
            {
                try
                {
                    var value = this.GetParameterFromRequestData(request, bindingModelProperty.Name);

                    bindingModelProperty.SetValue(bindingModelInstance, Convert.ChangeType(value, bindingModelProperty.PropertyType));
                }
                catch
                {
                    Console.WriteLine(string.Format(PropertyNotMappedMessage, bindingModelProperty.Name));
                }
            }

            return Convert.ChangeType(bindingModelInstance, bindingModelType);
        }

        private object ProcessPrimitiveParameter(ParameterInfo actionParameter, IHttpRequest request)
        {
            var value = this.GetParameterFromRequestData(request, actionParameter.Name);

            if (value == null)
            {
                return value;
            }

            return Convert.ChangeType(value, actionParameter.ParameterType);
        }

        private object GetParameterFromRequestData(IHttpRequest request, string actionParameterName)
        {
            if (request.QueryData.ContainsKey(actionParameterName))
            {
                return request.QueryData[actionParameterName];
            }

            if (request.FormData.ContainsKey(actionParameterName))
            {
                return request.FormData[actionParameterName];
            }

            return null;
        }

        private Controller GetController(string controllerName)
        {
            if (string.IsNullOrWhiteSpace(controllerName))
            {
                return null;
            }

            var fullyQualifiedControllerName = string.Format("{0}.{1}.{2}{3}, {0}", MvcContext.Get.AssemblyName, MvcContext.Get.ControllersFolder,
                controllerName,
                MvcContext.Get.ControllersSuffix);

            var controllerType = Type.GetType(fullyQualifiedControllerName);
            var controller = (Controller)this.dependencyContainer.CreateInstance(controllerType);

            return controller;
        }

        private MethodInfo GetMethod(string requestMethod, Controller controller, string actionName)
        {
            var actions = this
                .GetSuitableMethods(controller, actionName)
                .ToList();

            if (!actions.Any())
            {
                return null;
            }

            foreach (var action in actions)
            {
                var httpMethodAttributes = action
                    .GetCustomAttributes()
                    .Where(ca => ca is HttpMethodAttribute)
                    .Cast<HttpMethodAttribute>()
                    .ToList();

                if (!httpMethodAttributes.Any() &&
                    requestMethod.ToLower() == "get")
                {
                    return action;
                }

                foreach (var httpMethodAttribute in httpMethodAttributes)
                {
                    if (httpMethodAttribute.IsValid(requestMethod))
                    {
                        return action;
                    }
                }
            }

            return null;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods(Controller controller, string actionName)
        {
            if (controller == null)
            {
                return new MethodInfo[0];
            }

            return controller
                .GetType()
                .GetMethods()
                .Where(mi => mi.Name.ToLower() == actionName.ToLower());
        }

        private IHttpResponse PrepareResponse(IActionResult actionResult)
        {
            string invokeAtionResult = actionResult.Invoke();

            if (actionResult is IViewable)
            {
                return new HtmlResult(invokeAtionResult, HttpResponseStatusCode.Ok);
            }

            if (actionResult is IRedirectable)
            {
                return new RedirectResult(invokeAtionResult);
            }

            throw new InvalidOperationException(ResultTypeNotSupportedMessage);
        }

        private IHttpResponse Authorize(Controller controller, MethodInfo action)
        {
            if (action.GetCustomAttributes().Where(a => a is AuthorizeAttribute).Cast<AuthorizeAttribute>().Any(a => !a.IsAuthorized(controller.Identity)))
            {
                return new UnauthorizedResult();
            }

            return null;
        }
    }
}