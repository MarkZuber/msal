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
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Identity.Client
{
    public sealed partial class PublicClientApplication : ClientApplicationBase
    {
        /// <summary>
        ///     Non-interactive request to acquire a security token from the authority, via Username/Password Authentication.
        ///     Available only on .net desktop and .net core. See https://aka.ms/msal-net-up for details.
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="username">
        ///     Identifier of the user application requests token on behalf.
        ///     Generally in UserPrincipalName (UPN) format, e.g. john.doe@contoso.com
        /// </param>
        /// <param name="securePassword">User password.</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        public async Task<AuthenticationResult> AcquireTokenByUsernamePasswordAsync(
            IEnumerable<string> scopes,
            string username,
            SecureString securePassword)
        {
            var authParameters = new AuthenticationParameters
            {
                AuthorizationType = AuthorizationType.UsernamePassword,
                UserName = username,
                Password = SecureStringToNonSecure(securePassword)
            };
            authParameters.AddScopes(scopes);

            return await AcquireTokenSilentlyAsync(authParameters, CancellationToken.None).ConfigureAwait(false);
        }

        private string SecureStringToNonSecure(SecureString secureString)
        {
            var output = new char[secureString.Length];
            IntPtr secureStringPtr = Marshal.SecureStringToCoTaskMemUnicode(secureString);
            for (int i = 0; i < secureString.Length; i++)
            {
                output[i] = (char) Marshal.ReadInt16(secureStringPtr, i*2);
            }

            Marshal.ZeroFreeCoTaskMemUnicode(secureStringPtr);
            return new string(output);
        }

        //private async Task<AuthenticationResult> AcquireTokenByUsernamePasswordAsync(IEnumerable<string> scopes, UsernamePasswordInput usernamePasswordInput)
        //{
        //    Authority authority = Core.Instance.Authority.CreateAuthority(Authority, ValidateAuthority);
        //    var requestParams = CreateRequestParameters(authority, scopes, null, UserTokenCache);
        //    var handler = new UsernamePasswordRequest(HttpManager, CryptographyManager, WsTrustWebRequestManager, requestParams, usernamePasswordInput)
        //    {
        //        ApiId = ApiEvent.ApiIds.AcquireTokenWithScopeUser
        //    };

        //    return await handler.RunAsync(CancellationToken.None).ConfigureAwait(false);
        //}
    }
}