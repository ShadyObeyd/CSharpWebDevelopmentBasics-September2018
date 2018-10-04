namespace SIS.HTTP.Requests.Contracts
{
    using Cookies.Contracts;
    using Headers.Contracts;
    using Sessions.Contracts;
    using Enums;

    using System.Collections.Generic;

    public interface IHttpRequest
    {
        string Path { get; }

        string Url { get; }

        IHttpSession Session { get; set; }

        Dictionary<string, object> FormData { get; }

        Dictionary<string, object> QueryData { get; }

        IHttpHeaderCollection Headers { get; }

        IHttpCookieCollection Cookies { get; }

        HttpRequestMethod RequestMethod { get; }
    }
}
