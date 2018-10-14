using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Requests;

namespace Microsoft.Identity.Client.Cache
{
    internal interface ICacheManager
    {
        Task<TryReadCacheResponse> TryReadCache();
        Task<Account> CacheTokenResponseAsync(TokenResponse tokenResponse);
        Task DeleteCachedRefreshTokenAsync();
    }
}
