﻿namespace SIS.WebServer.Api.Contracts
{
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;

    public interface IHttpHandlingContext
    {
        IHttpResponse Handle(IHttpRequest request);
    }
}
