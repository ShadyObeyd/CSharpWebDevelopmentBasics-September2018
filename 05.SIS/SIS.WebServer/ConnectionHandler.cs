namespace SIS.WebServer
{
    using Routing;
    using Results;
    
    using HTTP.Responses.Contracts;
    using HTTP.Requests.Contracts;
    using HTTP.Requests;
    using HTTP.Responses;
    using HTTP.Enums;
    using HTTP.Sessions;
    using HTTP.Cookies;
    using HTTP.Common;

    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Text;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.IO;

    public class ConnectionHandler
    {
        private readonly Socket client;

        private readonly ServerRoutingTable serverRoutingTable;

        private const string DirectoryPath = "../../..";

        public ConnectionHandler(Socket client, ServerRoutingTable serverRoutingTable)
        {
            this.client = client;
            this.serverRoutingTable = serverRoutingTable;
        }

        private void SetResponseSession(IHttpResponse httpResponse, string sessionId)
        {
            if (sessionId != null)
            {
                var cookie = new HttpCookie(HttpSessionStorage.SessionCookieKey, $"{sessionId};HttpOnly=true");

                httpResponse.AddCookie(cookie);
            }
        }

        private string SetRequestSession(IHttpRequest httpRequest)
        {
            string sessionId = null;

            if (httpRequest.Cookies.ContainsCookie(HttpSessionStorage.SessionCookieKey))
            {
                var cookie = httpRequest.Cookies.GetCookie(HttpSessionStorage.SessionCookieKey);
                sessionId = cookie.Value;
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }
            else
            {
                sessionId = Guid.NewGuid().ToString();
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }

            return sessionId;
        }

        private async Task<IHttpRequest> ReadRequests()
        {
            var result = new StringBuilder();
            var data = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                int numberOfBytesRead = await this.client.ReceiveAsync(data.Array, SocketFlags.None);

                if (numberOfBytesRead == 0)
                {
                    break;
                }

                var bytesAsString = Encoding.UTF8.GetString(data.Array, 0, numberOfBytesRead);
                result.Append(bytesAsString);

                if (numberOfBytesRead < 1023)
                {
                    break;
                }
            }

            if (result.Length == 0)
            {
                return null;
            }

            return new HttpRequest(result.ToString());
        }

        private IHttpResponse HandleRequest(IHttpRequest request)
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

        private async Task PrepareResponse(IHttpResponse response)
        {
            byte[] byteSegments = response.GetBytes();

            await this.client.SendAsync(byteSegments, SocketFlags.None);
        }

        public async Task ProcessRequestAsync()
        {
            var httpRequest = await this.ReadRequests();

            if (httpRequest != null)
            {
                string sessionId = this.SetRequestSession(httpRequest);

                var httpResponse = this.HandleRequest(httpRequest);

                this.SetResponseSession(httpResponse, sessionId);

                await this.PrepareResponse(httpResponse);
            }

            this.client.Shutdown(SocketShutdown.Both);
        }
    }
}
