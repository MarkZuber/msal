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
using System.Runtime.Serialization;
using Microsoft.Identity.Client.Core;

namespace Microsoft.Identity.Client.Requests
{
    internal class TokenResponse
    {
        public TokenResponse(
            IdToken idToken,
            Credential accessToken,
            Credential refreshToken)
        {
            IdToken = idToken ?? new IdToken(string.Empty);
            if (accessToken != null)
            {
                AccessToken = accessToken.Secret;
                ExpiresOn = accessToken.ExpiresOn;
                ExtendedExpiresOn = accessToken.ExtendedExpiresOn;
                Scopes = ScopeUtils.Split(accessToken.Target);
            }

            if (refreshToken != null)
            {
                RefreshToken = refreshToken.Secret;
            }
        }

        internal TokenResponse(
            MsalTokenResponse mtr,
            ITimeService timeService = null)
        {
            var timeSvc = timeService ?? new TimeService();

            AccessToken = mtr.AccessToken;
            RefreshToken = mtr.RefreshToken;
            IdToken = string.IsNullOrWhiteSpace(mtr.IdToken) ? null : new IdToken(mtr.IdToken);
            Scopes = ScopeUtils.Split(mtr.Scope);
            ClientInfo clientInfo = string.IsNullOrWhiteSpace(mtr.ClientInfo) ? null : ClientInfo.Create(mtr.ClientInfo);

            ExpiresOn = timeSvc.GetUtcNow().AddSeconds(mtr.ExpiresIn);
            ExtendedExpiresOn = timeSvc.GetUtcNow().AddSeconds(mtr.ExtendedExpiresIn);

            Uid = clientInfo?.UniqueObjectIdentifier;
            Utid = clientInfo?.UniqueTenantIdentifier;
        }

        public string AccessToken { get; }
        public DateTime ExpiresOn { get; }
        public DateTime ExtendedExpiresOn { get; }
        public IEnumerable<string> Scopes { get; }
        public IdToken IdToken { get; }
        public string RefreshToken { get; }
        public string Uid { get; }
        public string Utid { get; }
        public bool HasAccessToken => !string.IsNullOrWhiteSpace(AccessToken);
        public bool HasRefreshToken => !string.IsNullOrWhiteSpace(RefreshToken);

        public static TokenResponse Create(HttpStatusCode httpStatusCode, string response)
        {
            var mtr = JsonHelper.DeserializeFromJson<MsalTokenResponse>(response);

            if (httpStatusCode != HttpStatusCode.OK || !string.IsNullOrWhiteSpace(mtr.Error))
            {
                // todo: exception
                throw new MsalException(mtr.Error, response);
            }

            if (string.IsNullOrWhiteSpace(mtr.AccessToken))
            {
                // todo: exception
                throw new InvalidOperationException();
            }

            return new TokenResponse(mtr);
        }

        // TODO: CLEAN THIS UP...
        internal class OAuth2ResponseBaseClaim
        {
            public const string Claims = "claims";
            public const string Error = "error";
            public const string ErrorDescription = "error_description";
            public const string ErrorCodes = "error_codes";
            public const string CorrelationId = "correlation_id";
        }

        [DataContract]
        internal class OAuth2ResponseBase
        {
            [DataMember(Name = OAuth2ResponseBaseClaim.Error, IsRequired = false)]
            public string Error { get; set; }

            [DataMember(Name = OAuth2ResponseBaseClaim.ErrorDescription, IsRequired = false)]
            public string ErrorDescription { get; set; }

            [DataMember(Name = OAuth2ResponseBaseClaim.ErrorCodes, IsRequired = false)]
            public string[] ErrorCodes { get; set; }

            [DataMember(Name = OAuth2ResponseBaseClaim.CorrelationId, IsRequired = false)]
            public string CorrelationId { get; set; }

            [DataMember(Name = OAuth2ResponseBaseClaim.Claims, IsRequired = false)]
            public string Claims { get; set; }
        }

        internal class TokenResponseClaim : OAuth2ResponseBaseClaim
        {
            public const string Code = "code";
            public const string TokenType = "token_type";
            public const string AccessToken = "access_token";
            public const string RefreshToken = "refresh_token";
            public const string IdToken = "id_token";
            public const string Scope = "scope";
            public const string ClientInfo = "client_info";
            public const string ExpiresIn = "expires_in";
            public const string CloudInstanceHost = "cloud_instance_host_name";
            public const string CreatedOn = "created_on";
            public const string ExtendedExpiresIn = "ext_expires_in";
            public const string Authority = "authority";
        }

        [DataContract]
        internal class MsalTokenResponse : OAuth2ResponseBase
        {
            [DataMember(Name = TokenResponseClaim.TokenType, IsRequired = false)]
            public string TokenType { get; set; }

            [DataMember(Name = TokenResponseClaim.AccessToken, IsRequired = false)]
            public string AccessToken { get; set; }

            [DataMember(Name = TokenResponseClaim.RefreshToken, IsRequired = false)]
            public string RefreshToken { get; set; }

            [DataMember(Name = TokenResponseClaim.Scope, IsRequired = false)]
            public string Scope { get; set; }

            [DataMember(Name = TokenResponseClaim.ClientInfo, IsRequired = false)]
            public string ClientInfo { get; set; }

            [DataMember(Name = TokenResponseClaim.IdToken, IsRequired = false)]
            public string IdToken { get; set; }

            [DataMember(Name = TokenResponseClaim.ExpiresIn, IsRequired = false)]
            public long ExpiresIn { get; set; }

            [DataMember(Name = TokenResponseClaim.ExtendedExpiresIn, IsRequired = false)]
            public long ExtendedExpiresIn { get; set; }

            public DateTimeOffset AccessTokenExpiresOn => DateTime.UtcNow + TimeSpan.FromSeconds(ExpiresIn);
        }
    }
}