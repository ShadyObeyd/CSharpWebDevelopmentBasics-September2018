namespace SIS.WebServer.Results
{
    using HTTP.Responses;
    using HTTP.Enums;
    using HTTP.Headers;

    using System;
    using System.Text;

    public class BadRequestResult : HttpResponse
    {
        private const string DefaultErrorHeading = "<h1>A server error occured!</h1>";

        private const string ContentType = "Content-Type";

        public BadRequestResult(string content, HttpResponseStatusCode responseStatusCode)
            : base(responseStatusCode)
        {
            content = DefaultErrorHeading + Environment.NewLine + content;
            this.Headers.Add(new HttpHeader(ContentType, "text/html"));
            this.Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
