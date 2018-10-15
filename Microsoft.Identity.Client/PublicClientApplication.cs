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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Browser;
using Microsoft.Identity.Client.Cache;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Http;
using Microsoft.Identity.Client.Platform;
using Microsoft.Identity.Client.Requests;
using Microsoft.Identity.Client.Telemetry;

namespace Microsoft.Identity.Client
{
    public class PublicClientApplication : IPublicClientApplication
    {
        private readonly IBrowserFactory _browserFactory;
        private readonly EnvironmentMetadata _environmentMetadata;
        private readonly IGuidService _guidService;
        private readonly IHttpManager _httpManager;
        private readonly IPlatformProxy _platformProxy;
        private readonly IStorageManager _storageManager;
        private readonly ITelemetryManager _telemetryManager;
        private readonly MsalClientConfiguration _msalClientConfiguration;

        public PublicClientApplication(MsalClientConfiguration msalClientConfiguration)
            : this(
                null,
                null,
                null,
                null,
                null,
                null,
                msalClientConfiguration)
        {
        }

        internal PublicClientApplication(
            IHttpManager httpManager,
            IStorageManager storageManager,
            IBrowserFactory browserFactory,
            IGuidService guidService,
            ITelemetryManager telemetryManager,
            EnvironmentMetadata environmentMetadata,
            MsalClientConfiguration msalClientConfiguration)
        {
            _platformProxy = PlatformProxyFactory.GetPlatformProxy();

            _httpManager = httpManager ?? new HttpManager(new HttpClientFactory(), msalClientConfiguration);
            _storageManager = storageManager ?? _platformProxy.CreateStorageManager();
            _browserFactory = browserFactory ?? _platformProxy.CreateBrowserFactory();
            _guidService = guidService ?? new GuidService();
            _telemetryManager = telemetryManager ?? new TelemetryManager(msalClientConfiguration);
            _environmentMetadata = environmentMetadata ?? new EnvironmentMetadata();
            _msalClientConfiguration = msalClientConfiguration;
        }

        /// <inheritdoc />
        public async Task<AuthenticationResult> SignInAsync(
            AuthenticationParameters authParameters,
            CancellationToken cancellationToken)
        {
            var request = CreateRequest(authParameters, true);
            return await request.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<AuthenticationResult> AcquireTokenInteractivelyAsync(
            AuthenticationParameters authParameters,
            CancellationToken cancellationToken)
        {
            return await SignInAsync(authParameters, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<AuthenticationResult> AcquireTokenSilentlyAsync(
            AuthenticationParameters authParameters,
            CancellationToken cancellationToken)
        {
            var request = CreateRequest(authParameters, false);
            return await request.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        private IMsalRequest CreateRequest(
            AuthenticationParameters authParameters,
            bool isInteractive)
        {
            var authParams = authParameters.Clone();

            authParams.TelemetryCorrelationId = _guidService.NewGuid();
            authParams.Logger = _platformProxy.CreateLogger(authParams.TelemetryCorrelationId, _msalClientConfiguration);

            var webRequestManager = new WebRequestManager(
                _httpManager,
                _platformProxy.GetSystemUtils(),
                _environmentMetadata,
                authParams);
            var cacheManager = new CacheManager(_storageManager, authParams);

            if (isInteractive)
            {
                return new MsalInteractiveRequest(webRequestManager, cacheManager, _browserFactory, authParams);
            }
            else
            {
                return new MsalBackgroundRequest(
                    webRequestManager,
                    cacheManager,
                    _platformProxy.GetSystemUtils(),
                    authParams);
            }
        }
    }
}