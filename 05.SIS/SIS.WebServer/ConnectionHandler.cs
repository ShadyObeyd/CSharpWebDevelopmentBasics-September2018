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
    using SIS.WebServer.Api.Contracts;

    public class ConnectionHandler
    {
        private readonly Socket client;

        private readonly IHttpHandler handler;

        private const string DirectoryPath = "../../..";

        public ConnectionHandler(Socket client, IHttpHandler handler)
        {
            this.client = client;
            this.handler = handler;
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

                var httpResponse = this.handler.Handle(httpRequest);

                this.SetResponseSession(httpResponse, sessionId);

                await this.PrepareResponse(httpResponse);
            }

            this.client.Shutdown(SocketShutdown.Both);
        }
    }
}