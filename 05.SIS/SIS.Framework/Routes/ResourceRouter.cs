namespace SIS.Framework.Routes
{
    using HTTP.Enums;
    using HTTP.Requests.Contracts;
    using HTTP.Responses;
    using HTTP.Responses.Contracts;
    using WebServer.Api.Contracts;
    using WebServer.Results;

    using System.IO;

    public class ResourceRouter : IHttpHandler
    {
        private const string DirectoryPath = "../../..";

        public IHttpResponse Handle(IHttpRequest request)
        {
            string requestPath = request.Path;

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
