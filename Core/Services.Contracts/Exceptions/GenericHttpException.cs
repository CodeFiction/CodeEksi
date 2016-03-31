using System;
using System.Net;

namespace Services.Contracts.Exceptions
{
    public class GenericHttpException : Exception
    {
        public GenericHttpException(HttpStatusCode httpStatusCode, string requestUrl)
            : base($"{httpStatusCode} - {requestUrl}")
        {
            RequestUrl = requestUrl;
            StatusCode = httpStatusCode;
        }

        public HttpStatusCode StatusCode { get; }

        public string RequestUrl { get; }
    }
}