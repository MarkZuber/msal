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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Http;
using Microsoft.Identity.Client.Platform;
using Microsoft.Identity.Client.Requests.WsTrust;

namespace Microsoft.Identity.Client.Requests
{
    internal static class OAuth2Scope
    {
        public const string OpenId = "openid";
        public const string OfflineAccess = "offline_access";
        public const string Profile = "profile";
    }

    internal static class ServerHeaderKey
    {
        public const string XClientSku = "x-client-SKU";
        public const string XClientOs = "x-client-OS";
        public const string XClientVer = "x-client-Ver";
    }

    internal class WebRequestManager
    {
        private readonly AuthenticationParameters _authenticationParameters;
        private readonly EnvironmentMetadata _environmentMetadata;
        private readonly IHttpManager _httpManager;
        private readonly ISystemUtils _systemUtils;

        public WebRequestManager(
            IHttpManager httpManager,
            ISystemUtils systemUtils,
            EnvironmentMetadata environmentMetadata,
            AuthenticationParameters authenticationParameters)
        {
            _httpManager = httpManager;
            _systemUtils = systemUtils;
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
            var response = await _httpManager.GetAsync(new Uri(url), GetVersionHeaders(), cancellationToken);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                // todo: exception type
                throw new Exception("User Realm Error");
            }

            return UserRealm.Create(response.ResponseData);
        }

        private IDictionary<string, string> GetVersionHeaders()
        {
            return new Dictionary<string, string>
            {
                [ServerHeaderKey.XClientSku] = _systemUtils.GetClientSku(),
                [ServerHeaderKey.XClientOs] = _systemUtils.GetOsVersion(),
                [ServerHeaderKey.XClientVer] = _systemUtils.GetProductVersion()
            };
        }

        public async Task<WsTrustMexDocument> GetMexAsync(
            string federationMetadataUrl,
            CancellationToken cancellationToken)
        {
            var response = await _httpManager.GetAsync(new Uri(federationMetadataUrl), null, cancellationToken);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                // todo: exception type
                throw new Exception("Mex Error");
            }

            return WsTrustMexDocument.Create(response.ResponseData);
        }

        public async Task<WsTrustResponse> GetWsTrustResponseAsync(
            string cloudAudienceUrn,
            WsTrustEndpoint endpoint,
            CancellationToken cancellationToken)
        {
            string wsTrustRequestMessage;
            var authorizationType = _authenticationParameters.AuthorizationType;
            switch (authorizationType)
            {
            case AuthorizationType.WindowsIntegratedAuth:
                wsTrustRequestMessage = endpoint.BuildTokenRequestMessageWIA(cloudAudienceUrn);
                break;
            case AuthorizationType.UsernamePassword:
                wsTrustRequestMessage = endpoint.BuildTokenRequestMessageUsernamePassword(
                    cloudAudienceUrn,
                    _authenticationParameters.UserName,
                    _authenticationParameters.Password);
                break;
            default:
                throw new InvalidOperationException();
            }

            string soapAction = endpoint.IsWsTrust2005()
                                    ? WsTrustMexDocument.Trust2005Spec
                                    : WsTrustMexDocument.Trust13Spec;

            var requestHeaders = new Dictionary<string, string>
            {
                ["SOAPAction"] = soapAction,
                ["Content-Type"] = "application/soap+xml; charset=utf-8"
            };

            var response = await _httpManager
                                 .PostAsync(endpoint.Url, requestHeaders, wsTrustRequestMessage, cancellationToken)
                                 .ConfigureAwait(false);
            return WsTrustResponse.Create(response.ResponseData);
        }

        public async Task<TokenResponse> GetAccessTokenFromSamlGrantAsync(
            SamlTokenInfo samlGrant,
            CancellationToken cancellationToken)
        {
            QueryParameterBuilder queryParams;
            switch (samlGrant.GetAssertionType())
            {
            case SamlAssertionType.SamlV1:
                queryParams = new QueryParameterBuilder("grant_type", "urn:ietf:params:oauth:grant-type:saml1_1-bearer");
                break;
            case SamlAssertionType.SamlV2:
                queryParams = new QueryParameterBuilder("grant_type", "urn:ietf:params:oauth:grant-type:saml2-bearer");
                break;
            default:
                throw new InvalidOperationException(); // (MsalXmlException, MSAL_SAML_ENUM_UNKNOWN_VERSION);
            }

            queryParams.AddQueryPair("assertion", _authenticationParameters.UserName);
            queryParams.AddQueryPair("password", EncodingUtils.Base64RfcEncodePadded(samlGrant.GetAssertion()));
            queryParams.AddClientIdQueryParam();
            queryParams.AddScopeQueryParam();
            queryParams.AddClientInfoQueryParam();

            var headers = GetVersionHeaders();
            headers["Content-Type"] = "application/x-www-form-urlencoded";

            var response = await _httpManager.PostAsync(
                               _authenticationParameters.Authority.GetTokenEndpoint(),
                               headers,
                               queryParams.ToString(),
                               cancellationToken).ConfigureAwait(false);

            return TokenResponse.Create(response.ResponseData);
        }

        public async Task<TokenResponse> GetAccessTokenFromUsernamePasswordAsync(CancellationToken cancellationToken)
        {
            var queryParams = new QueryParameterBuilder("grant_type", "password");
            queryParams.AddQueryPair("username", _authenticationParameters.UserName);
            queryParams.AddQueryPair("password", _authenticationParameters.Password);
            queryParams.AddClientIdQueryParam();
            queryParams.AddScopeQueryParam();
            queryParams.AddClientInfoQueryParam();

            var response = await _httpManager.PostAsync(
                               _authenticationParameters.Authority.GetTokenEndpoint(),
                               GetVersionHeaders(),
                               queryParams.ToString(),
                               cancellationToken).ConfigureAwait(false);
            return TokenResponse.Create(response.ResponseData);
        }

        public async Task<TokenResponse> GetAccessTokenFromAuthCodeAsync(
            string authCode,
            CancellationToken cancellationToken)
        {
            var queryParams = new QueryParameterBuilder("grant_type", "authorization_code");
            queryParams.AddQueryPair("code", authCode);
            queryParams.AddRedirectUriQueryParam();
            queryParams.AddClientIdQueryParam();
            queryParams.AddScopeQueryParam();
            queryParams.AddClientInfoQueryParam();

            var response = await _httpManager.PostAsync(
                               _authenticationParameters.Authority.GetTokenEndpoint(),
                               GetVersionHeaders(),
                               queryParams.ToString(),
                               cancellationToken).ConfigureAwait(false);
            return TokenResponse.Create(response.ResponseData);
        }

        public async Task<TokenResponse> GetAccessTokenFromRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken)
        {
            var queryParams = new QueryParameterBuilder("grant_type", "refresh_token");
            queryParams.AddQueryPair("refresh_token", refreshToken);
            queryParams.AddClientIdQueryParam();
            queryParams.AddScopeQueryParam();
            queryParams.AddClientInfoQueryParam();

            var response = await _httpManager.PostAsync(
                               _authenticationParameters.Authority.GetTokenEndpoint(),
                               GetVersionHeaders(),
                               queryParams.ToString(),
                               cancellationToken).ConfigureAwait(false);
            return TokenResponse.Create(response.ResponseData);
        }
    }
}