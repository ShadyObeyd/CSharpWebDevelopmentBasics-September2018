namespace SIS.WebServer.Results
{
    using HTTP.Responses;
    using HTTP.Enums;
    using HTTP.Headers;

    using System.Text;

    public class UnauthorizedResult : HttpResponse
    {
        private const string DefaultErrorHeading = "<h1>You have no permission to access this functionality.</h1>";

        public UnauthorizedResult()
            : base(HttpResponseStatusCode.Unauthorized)
        {
            this.Headers.Add(new HttpHeader("Content-Type", "text/html"));
            this.Content = Encoding.UTF8.GetBytes(DefaultErrorHeading);
        }
    }
}
