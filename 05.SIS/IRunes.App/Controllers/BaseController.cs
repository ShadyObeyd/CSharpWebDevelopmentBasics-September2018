﻿namespace IRunes.App.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;

    using Data;
    using Services;

    using SIS.HTTP.Cookies;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;

    public abstract class BaseController
    {
        private const string DirectoryPath = "../../../";

        private const string ViewFolderName = "Views";

        private const string ControllerDefaultName = "Controller";

        private const string HtmlFileExtension = ".html";

        private const string FileNotFoundMessage = "File with path {0} was not found";

        private const string LayoutFileName = "_layout";

        private const string RenderBodyConstant = "@RenderBody()";

        protected IRunesContext Context { get; }

        protected IDictionary<string, string> ViewBag { get; set; }

        public BaseController()
        {
            this.Context = new IRunesContext();
            this.ViewBag = new Dictionary<string, string>();
        }

        public bool IsAuthenticated(IHttpRequest request)
        {
            return request.Session.ContainsParameter("username");
        }

        protected IHttpResponse View([CallerMemberName] string viewName = "")
        {
            var layoutView = $"{DirectoryPath}{ViewFolderName}/{LayoutFileName}{HtmlFileExtension}";

            string filePath = $"{DirectoryPath}{ViewFolderName}/{this.GetControllerName()}/{viewName}{HtmlFileExtension}";

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format(FileNotFoundMessage, filePath));
            }

            string viewContent = BuildViewContent(filePath);

            var viewLayout = File.ReadAllText(layoutView);

            var view = viewLayout.Replace(RenderBodyConstant, viewContent);

            var response = new HtmlResult(view, HttpResponseStatusCode.Ok);

            return response;
        }

        private string BuildViewContent(string filePath)
        {
            string viewContent = File.ReadAllText(filePath);

            foreach (string viewBagKey in ViewBag.Keys)
            {
                if (viewContent.Contains($"{{{viewBagKey}}}"))
                {
                    viewContent = viewContent.Replace($"{{{{{viewBagKey}}}}}", this.ViewBag[viewBagKey]);
                }
            }

            return viewContent;
        }

        public void SignInUser(string username, IHttpRequest request)
        {
            string cookieValue = new UserCookieService().GetUserCookie(username);

            request.Session.AddParameter("username", username);
            request.Cookies.Add(new HttpCookie("IRunes_auth", cookieValue));

        }

        private string GetControllerName()
        {
            return this.GetType().Name.Replace(ControllerDefaultName, string.Empty);
        }
    }
}