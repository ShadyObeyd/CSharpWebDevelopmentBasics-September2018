namespace SIS.WebServer.Api
{
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using HTTP.Responses;
    using HTTP.Enums;
    using HTTP.Common;

    using Contracts;
    using Routing;
    using Results;

    using System.IO;
    using System.Linq;

    public class HttpHandler : IHttpHandler
    {
        private readonly ServerRoutingTable serverRoutingTable;

        private const string DirectoryPath = "../../..";

        public HttpHandler(ServerRoutingTable serverRoutingTable)
        {
            this.serverRoutingTable = serverRoutingTable;
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            bool isResaurceRequest = this.IsResaurceRequest(request);

            if (isResaurceRequest)
            {
                return this.HandleRequestRespons(request.Path);
            }

            if (!this.serverRoutingTable.Routes.ContainsKey(request.RequestMethod) ||
                !this.serverRoutingTable.Routes[request.RequestMethod].ContainsKey(request.Path))
            {
                return new HttpResponse(HttpResponseStatusCode.NotFound);
            }

            return this.serverRoutingTable.Routes[request.RequestMethod][request.Path].Invoke(request);
        }

        private bool IsResaurceRequest(IHttpRequest request)
        {
            string requestPath = request.Path;

            if (requestPath.Contains("."))
            {
                string requestPathExtension = requestPath.Substring(requestPath.LastIndexOf('.'));

                return GlobalConstants.ResourceExtensions.Contains(requestPathExtension);
            }

            return false;

        }

        private IHttpResponse HandleRequestRespons(string requestPath)
        {
            string requestPathExtension = requestPath.Substring(requestPath.LastIndexOf('.'));

            var resourceName = requestPath.Substring(requestPath.LastIndexOf('/'));

            string resourcePath = DirectoryPath + "/Resources" + $"/{requestPathExtension.Substring(1)}" + resourceName;

            if (!File.Exists(resourcePath))
            {
                return new HttpResponse(HttpResponseStatusCode.NotFound);
            }

            var fileContent = File.ReadAllBytes(resourcePath);

            return new InlineResourceResult(fileContent, HttpResponseStatusCode.Ok);
        }
    }
}