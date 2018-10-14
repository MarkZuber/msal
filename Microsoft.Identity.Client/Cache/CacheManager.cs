using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Requests;

namespace Microsoft.Identity.Client.Cache
{
    internal class CacheManager : ICacheManager
    {
        private readonly IStorageManager _storageManager;
        private readonly AuthenticationParameters _authenticationParameters;

        public CacheManager(IStorageManager storageManager, AuthenticationParameters authenticationParameters)
        {
            _storageManager = storageManager;
            _authenticationParameters = authenticationParameters;
        }

        /// <inheritdoc />
        public Task<TryReadCacheResponse> TryReadCache()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<Account> CacheTokenResponseAsync(TokenResponse tokenResponse)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task DeleteCachedRefreshTokenAsync()
        {
            throw new NotImplementedException();
        }
    }
}
