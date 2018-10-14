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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Http;
using Microsoft.Identity.Client.Requests.WsTrust;

namespace Microsoft.Identity.Client.Requests
{
    internal static class OAuth2Scope
    {
        public static readonly string OpenId = "openid";
        public static readonly string OfflineAccess = "offline_access";
        public static readonly string Profile = "profile";
    }

    internal class WebRequestManager
    {
        private readonly AuthenticationParameters _authenticationParameters;
        private readonly EnvironmentMetadata _environmentMetadata;
        private readonly IHttpManager _httpManager;

        public WebRequestManager(
            IHttpManager httpManager,
            EnvironmentMetadata environmentMetadata,
            AuthenticationParameters authenticationParameters)
        {
            _httpManager = httpManager;
            _environmentMetadata = environmentMetadata;
            _authenticationParameters = authenticationParameters;

            _authenticationParameters.AddScope(OAuth2Scope.OpenId);
            _authenticationParameters.AddScope(OAuth2Scope.OfflineAccess);
            _authenticationParameters.AddScope(OAuth2Scope.Profile);
        }

        public Task<CloudEnvironmentInfo> GetCloudEnvironmentInfoAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<UserRealm> GetUserRealmAsync(CancellationToken cancellationToken)
        {
            string url = _authenticationParameters.Authority.GetUserRealmEndpoint(_authenticationParameters.UserName);
            HttpManagerResponse response = await _httpManager.GetAsync(new Uri(url), GetVersionHeaders(), cancellationToken);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                // todo: exception type
                throw new Exception("User Realm Error");
            }

            return UserRealm.Create(response.ResponseData);
        }

        public async Task<WsTrustMexDocument> GetMexAsync(string federationMetadataUrl, CancellationToken cancellationToken)
        {
            HttpManagerResponse response = await _httpManager.GetAsync(new Uri(federationMetadataUrl), null, cancellationToken);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                // todo: exception type
                throw new Exception("Mex Error");
            }

            return WsTrustMexDocument.Create(response.ResponseData);
        }

        public Task<WsTrustResponse> GetWsTrustResponseAsync(string cloudAudienceUrn, WsTrustEndpoint endpoint, CancellationToken cancellationToken)
        {
        }

        public Task<TokenResponse> GetAccessTokenFromSamlGrantAsync(SamlTokenInfo samlGrant, CancellationToken cancellationToken)
        {
        }

        public Task<TokenResponse> GetAccessTokenFromUsernamePasswordAsync(CancellationToken cancellationToken)
        {
        }

        public Task<TokenResponse> GetAccessTokenFromAuthCodeAsync(string authCode, CancellationToken cancellationToken)
        {
        }

        public Task<TokenResponse> GetAccessTokenFromRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
        }
    }
}