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
using Microsoft.Identity.Client.Requests;

namespace Microsoft.Identity.Client
{
    public class AuthenticationResult
    {
        private AuthenticationResult(
            Account account,
            IdToken idToken,
            string accessToken,
            DateTime expiresOn,
            IEnumerable<string> scopes)
        {
            Account = account;
            IdToken = idToken == null ? string.Empty : idToken.Payload;
            AccessToken = accessToken;
            ExpiresOn = expiresOn;
            Scopes = scopes;
        }

        private AuthenticationResult()
        {
            IsCanceled = true;
        }

        private AuthenticationResult(int errorCode)
        {
            ErrorCode = errorCode;
            IsError = true;
        }

        public bool IsCanceled { get; }
        public bool IsError { get; }
        public int ErrorCode { get; }
        public Account Account { get; }
        public string IdToken { get; }
        public string AccessToken { get; }
        public DateTime ExpiresOn { get; }
        public IEnumerable<string> Scopes { get; }
        public IEnumerable<string> DeclinedScopes => throw new NotImplementedException();

        internal static AuthenticationResult Create(
            TokenResponse response,
            Account account)
        {
            return new AuthenticationResult(
                account,
                response.IdToken,
                response.AccessToken,
                response.ExpiresOn,
                response.Scopes);
        }

        internal static AuthenticationResult CreateCanceled()
        {
            return new AuthenticationResult();
        }

        internal static AuthenticationResult CreateWithError(int errorCode)
        {
            return new AuthenticationResult(errorCode);
        }
    }
}