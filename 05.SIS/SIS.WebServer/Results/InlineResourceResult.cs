namespace SIS.WebServer.Results
{
    using HTTP.Responses;
    using HTTP.Enums;
    using HTTP.Headers;

    public class InlineResourceResult : HttpResponse
    {
        private const string ContentLenghtHeader = "Content-Length";
        private const string ContentDispositionHeader = "Content-Disposition";
        private const string InlineValue = "inline";

        public InlineResourceResult(byte[] content, HttpResponseStatusCode statusCode)
            : base(statusCode)
        {
            this.Headers.Add(new HttpHeader(ContentLenghtHeader, content.Length.ToString()));
            this.Headers.Add(new HttpHeader(ContentDispositionHeader, InlineValue));
            this.Content = content;
        }
    }
}
