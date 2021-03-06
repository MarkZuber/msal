﻿// ------------------------------------------------------------------------------
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
using System.Threading.Tasks;

namespace Microsoft.Identity.Client
{
    public sealed partial class PublicClientApplication : ClientApplicationBase
    {
        // .net core does not support getting the upn from the OS, although it can be made to pull it from a Windows OS
#if !NET_CORE
        /// <summary>
        ///     Non-interactive request to acquire a security token for the signed-in user in Windows, via Integrated Windows
        ///     Authentication.
        ///     See https://aka.ms/msal-net-iwa.
        ///     The account used in this overrides is pulled from the operating system as the current user principal name
        /// </summary>
        /// <remarks>
        ///     On Windows Universal Platform, the following capabilities need to be provided:
        ///     Enterprise Authentication, Private Networks (Client and Server), User Account Information
        ///     Supported on .net desktop and UWP
        /// </remarks>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <returns>
        ///     Authentication result containing a token for the requested scopes and for the currently logged-in user in
        ///     Windows
        /// </returns>
        public Task<AuthenticationResult> AcquireTokenByIntegratedWindowsAuthAsync(IEnumerable<string> scopes)
        {
            throw new NotImplementedException();
            //return await AcquireTokenByIWAAsync(scopes, new IntegratedWindowsAuthInput()).ConfigureAwait(false);
        }
#endif

        /// <summary>
        ///     Non-interactive request to acquire a security token for the signed-in user in Windows, via Integrated Windows
        ///     Authentication.
        ///     See https://aka.ms/msal-net-iwa.
        ///     The account used in this overrides is pulled from the operating system as the current user principal name
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="username">
        ///     Identifier of the user account for which to acquire a token with Integrated Windows authentication.
        ///     Generally in UserPrincipalName (UPN) format, e.g. john.doe@contoso.com
        /// </param>
        /// <returns>
        ///     Authentication result containing a token for the requested scopes and for the currently logged-in user in
        ///     Windows
        /// </returns>
        public Task<AuthenticationResult> AcquireTokenByIntegratedWindowsAuthAsync(
            IEnumerable<string> scopes,
            string username)
        {
            throw new NotImplementedException();
            //return await AcquireTokenByIWAAsync(scopes, new IntegratedWindowsAuthInput(username)).ConfigureAwait(false);
        }

        //private Task<AuthenticationResult> AcquireTokenByIWAAsync(IEnumerable<string> scopes, IntegratedWindowsAuthInput iwaInput)
        //{
        //    throw new NotImplementedException();
        //    //Authority authority = Core.Instance.Authority.CreateAuthority(Authority, ValidateAuthority);
        //    //var requestParams = CreateRequestParameters(authority, scopes, null, UserTokenCache);
        //    //var handler = new IntegratedWindowsAuthRequest(HttpManager, CryptographyManager, WsTrustWebRequestManager, requestParams, iwaInput)
        //    //{
        //    //    ApiId = ApiEvent.ApiIds.AcquireTokenWithScopeUser
        //    //};

        //    //return await handler.RunAsync(CancellationToken.None).ConfigureAwait(false);
        //}
    }
}