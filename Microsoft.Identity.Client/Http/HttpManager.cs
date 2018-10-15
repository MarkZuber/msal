// ------------------------------------------------------------------------------
// 
// Copyright (c) Microsoft Corporation.
// All rights reserved.
// 
// This code is licensed under the MIT License.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Identity.Client.Http
{
    internal class HttpManager : IHttpManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MsalClientConfiguration _msalClientConfiguration;

        public HttpManager(
            IHttpClientFactory httpClientFactory,
            MsalClientConfiguration clientConfiguration)
        {
            _httpClientFactory = httpClientFactory;
            _msalClientConfiguration = clientConfiguration;
        }

        /// <inheritdoc />
        public async Task<HttpManagerResponse> GetAsync(
            Uri uri,
            IDictionary<string, string> requestHeaders,
            CancellationToken cancellationToken)
        {
            return await SendHttpRequestWithRetryAsync(
                       HttpMethod.Get,
                       uri,
                       requestHeaders,
                       null,
                       cancellationToken);
        }

        /// <inheritdoc />
        public async Task<HttpManagerResponse> PostAsync(
            Uri uri,
            IDictionary<string, string> requestHeaders,
            string body,
            CancellationToken cancellationToken)
        {
            return await SendHttpRequestWithRetryAsync(
                       HttpMethod.Post,
                       uri,
                       requestHeaders,
                       body,
                       cancellationToken);
        }

        private async Task<HttpManagerResponse> SendHttpRequestWithRetryAsync(
            HttpMethod httpMethod,
            Uri uri,
            IDictionary<string, string>
                requestHeaders,
            string body,
            CancellationToken cancellationToken)
        {
            HttpManagerResponse httpManagerResponse = null;
            var retry = new RetryWithExponentialBackoff();
            await retry.RunAsync(
                async () =>
                {
                    httpManagerResponse = await ExecuteAsync(
                                              httpMethod,
                                              uri,
                                              requestHeaders,
                                              body == null ? null : new StringContent(body),
                                              cancellationToken).ConfigureAwait(false);
                }).ConfigureAwait(false);

            return httpManagerResponse;
        }

        private async Task<HttpManagerResponse> ExecuteAsync(
            HttpMethod httpMethod,
            Uri uri,
            IDictionary<string, string> requestHeaders,
            HttpContent body,
            CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.GetHttpClient(_msalClientConfiguration);
            using (var requestMessage = CreateRequestMessage(uri, requestHeaders))
            {
                requestMessage.Method = httpMethod;
                requestMessage.Content = body;

                using (var responseMessage =
                    await httpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false))
                {
                    return await CreateResponseAsync(
                               responseMessage,
                               httpClient.DefaultRequestHeaders.UserAgent.ToString()).ConfigureAwait(false);
                }
            }
        }

        private async Task<HttpManagerResponse> CreateResponseAsync(
            HttpResponseMessage response,
            string userAgent)
        {
            return new HttpManagerResponse(
                response.StatusCode,
                await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                response.Headers,
                userAgent);
        }

        private HttpRequestMessage CreateRequestMessage(
            Uri uri,
            IDictionary<string, string> headers)
        {
            var message = new HttpRequestMessage
            {
                RequestUri = uri
            };

            message.Headers.Accept.Clear();
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> kvp in headers)
                {
                    message.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            return message;
        }
    }
}