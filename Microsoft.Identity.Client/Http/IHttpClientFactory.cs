using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Microsoft.Identity.Client.Http
{
    internal interface IHttpClientFactory
    {
        HttpClient GetHttpClient(MsalClientConfiguration clientConfiguration);
    }
}
