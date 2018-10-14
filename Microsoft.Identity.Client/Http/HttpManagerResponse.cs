using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Identity.Client.Core;

namespace Microsoft.Identity.Client.Http
{
    internal class HttpManagerResponse
    {
        public HttpManagerResponse(
            HttpStatusCode statusCode,
            string responseData,
            HttpResponseHeaders headers,
            string userAgent)
        {
            StatusCode = statusCode;
            ResponseData = responseData;
            Headers = headers;
            UserAgent = userAgent;
        }

        public HttpStatusCode StatusCode { get; }
        public string ResponseData { get; }
        public HttpResponseHeaders Headers { get; }
        public string UserAgent { get; }
    }
}
