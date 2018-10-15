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
using Microsoft.Identity.Client.Cache;
using Microsoft.Identity.Client.Platform;

namespace Microsoft.Identity.Client.Requests
{
    internal class MsalBackgroundRequest : IMsalRequest
    {
        private readonly AuthenticationParameters _authParameters;
        private readonly CacheManager _cacheManager;
        private readonly ISystemUtils _systemUtils;
        private readonly WebRequestManager _webRequestManager;

        public MsalBackgroundRequest(
            WebRequestManager webRequestManager,
            CacheManager cacheManager,
            ISystemUtils systemUtils,
            AuthenticationParameters authParameters)
        {
            _webRequestManager = webRequestManager;
            _cacheManager = cacheManager;
            _authParameters = authParameters;
            _systemUtils = systemUtils;
        }

        /// <inheritdoc />
        public async Task<AuthenticationResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            if (_authParameters.AuthorizationType != AuthorizationType.AuthCode)
            {
                var cacheResult = await TryReadCacheAsync(cancellationToken).ConfigureAwait(false);
                if (cacheResult.IsCacheReadSuccessful)
                {
                    return AuthenticationResult.Create(cacheResult.TokenResponse, cacheResult.Account);
                }
            }

            TokenResponse tokenResponse;

            switch (_authParameters.AuthorizationType)
            {
            case AuthorizationType.AuthCode:
                tokenResponse = await AuthCodeExchangeAsync(cancellationToken).ConfigureAwait(false);
                break;
            case AuthorizationType.UsernamePassword:
                tokenResponse = await UsernamePasswordExchangeAsync(cancellationToken).ConfigureAwait(false);
                break;
            case AuthorizationType.WindowsIntegratedAuth:
                tokenResponse = await WindowsIntegratedAuthExchangeAsync(cancellationToken).ConfigureAwait(false);
                break;
            case AuthorizationType.None:
                throw new InvalidOperationException("msal background request called with None type");
            default:
                throw new InvalidOperationException("unknown or unsupported auth type");
            }

            var account = await _cacheManager.CacheTokenResponseAsync(tokenResponse).ConfigureAwait(false);
            return AuthenticationResult.Create(tokenResponse, account);
        }

        private async Task<TokenResponse> AuthCodeExchangeAsync(CancellationToken cancellationToken)
        {
            // todo: check for embedded browser result...

            string authCode = string.Empty;
            return await _webRequestManager.GetAccessTokenFromAuthCodeAsync(authCode, cancellationToken)
                                           .ConfigureAwait(false);
        }

        private async Task<TokenResponse> RefreshTokenExchangeAsync(
            string refreshToken,
            CancellationToken cancellationToken)
        {
            return await _webRequestManager.GetAccessTokenFromRefreshTokenAsync(refreshToken, cancellationToken)
                                           .ConfigureAwait(false);
        }

        private async Task<TokenResponse> UsernamePasswordExchangeAsync(CancellationToken cancellationToken)
        {
            var userRealm = await _webRequestManager.GetUserRealmAsync(cancellationToken).ConfigureAwait(false);
            if (userRealm.IsFederated)
            {
                var mexDoc = await _webRequestManager.GetMexAsync(userRealm.FederationMetadataUrl, cancellationToken)
                                                     .ConfigureAwait(false);
                var wsTrustResponse = await _webRequestManager.GetWsTrustResponseAsync(
                                          userRealm.CloudAudienceUrn,
                                          mexDoc.GetWsTrustUsernamePasswordEndpoint(),
                                          cancellationToken).ConfigureAwait(false);
                var samlGrant = wsTrustResponse.GetSamlAssertion(mexDoc.GetWsTrustUsernamePasswordEndpoint());
                return await _webRequestManager.GetAccessTokenFromSamlGrantAsync(samlGrant, cancellationToken)
                                               .ConfigureAwait(false);
            }
            else
            {
                return await _webRequestManager.GetAccessTokenFromUsernamePasswordAsync(cancellationToken)
                                               .ConfigureAwait(false);
            }
        }

        private async Task<TokenResponse> WindowsIntegratedAuthExchangeAsync(CancellationToken cancellationToken)
        {
            // todo: throw if not on a windows system
            // todo: even better, implement capabilities check system...

            string username = _systemUtils.GetCurrentUsername();
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException("failed to retrieve current user name");
            }

            _authParameters.UserName = username;

            var userRealm = await _webRequestManager.GetUserRealmAsync(cancellationToken).ConfigureAwait(false);
            if (!userRealm.IsFederated)
            {
                throw new InvalidOperationException("wia only supports federated users");
            }

            var mexDoc = await _webRequestManager.GetMexAsync(userRealm.FederationMetadataUrl, cancellationToken)
                                                 .ConfigureAwait(false);
            var wsTrustResponse = await _webRequestManager.GetWsTrustResponseAsync(
                                      userRealm.CloudAudienceUrn,
                                      mexDoc.GetWsTrustWindowsTransportEndpoint(),
                                      cancellationToken).ConfigureAwait(false);
            var samlGrant = wsTrustResponse.GetSamlAssertion(mexDoc.GetWsTrustWindowsTransportEndpoint());
            return await _webRequestManager.GetAccessTokenFromSamlGrantAsync(samlGrant, cancellationToken)
                                           .ConfigureAwait(false);
        }

        private Task<TryReadCacheResponse> TryReadCacheAsync(CancellationToken cancellationToken)
        {
            // todo: implement cache
            return Task.FromResult(new TryReadCacheResponse(false, null, null));
        }
    }
}