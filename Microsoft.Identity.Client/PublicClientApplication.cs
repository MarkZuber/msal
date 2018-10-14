using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Browser;
using Microsoft.Identity.Client.Cache;
using Microsoft.Identity.Client.Http;
using Microsoft.Identity.Client.Requests;

namespace Microsoft.Identity.Client
{
    public class PublicClientApplication : IPublicClientApplication
    {
        private readonly IHttpManager _httpManager;
        private readonly EnvironmentMetadata _environmentMetadata;
        private readonly IStorageManager _storageManager;
        private readonly IBrowserFactory _browserFactory;
        private readonly IRequestDispatcher _requestDispatcher;

        /// <inheritdoc />
        public async Task<AuthenticationResult> SignInAsync(AuthenticationParameters authParameters)
        {
            var webRequestManager = new WebRequestManager(_httpManager, _environmentMetadata, authParameters);
            var cacheManager = new CacheManager(_storageManager, authParameters);
            var request = new MsalInteractiveRequest(
                _requestDispatcher,
                webRequestManager,
                cacheManager,
                _browserFactory,
                authParameters);
            return await _requestDispatcher.ExecuteInteractiveRequestAsync(request);
        }

        /// <inheritdoc />
        public async Task<AuthenticationResult> AcquireTokenInteractivelyAsync(AuthenticationParameters authParameters)
        {
            return await SignInAsync(authParameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<AuthenticationResult> AcquireTokenSilentlyAsync(AuthenticationParameters authParameters)
        {
            var webRequestManager = new WebRequestManager(_httpManager, _environmentMetadata, authParameters);
            var cacheManager = new CacheManager(_storageManager, authParameters);
            var request = new MsalBackgroundRequest(webRequestManager, cacheManager, authParameters);
            return await _requestDispatcher.ExecuteBackgroundRequestAsync(request).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task ShutdownAsync()
        {
            await _requestDispatcher.ShutdownAsync();
        }
    }
}
