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

using System.Net;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Identity.Client.TestInfrastructure;
using Microsoft.Identity.Client.TestInfrastructure.Labs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Identity.Client.IntegrationTests
{
    [TestClass]
    public class UsernamePasswordIntegrationTests
    {
        private const string ClientId = "0615b6ca-88d4-4884-8729-b178178f7c27";
        private const string Authority = "https://login.microsoftonline.com/organizations/";
        private readonly AuthHelper _authHelper = new AuthHelper();

        private readonly string[] _scopes =
        {
            "User.Read"
        };

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        [TestMethod]
        [TestCategory("UsernamePasswordIntegrationTests")]
        public async Task AcquireTokenWithManagedUsernamePasswordAsync()
        {
            var user = _authHelper.GetUser(
                new UserQueryParameters
                {
                    IsMamUser = false,
                    IsMfaUser = false,
                    IsFederatedUser = false
                });

            var securePassword = new NetworkCredential("", ((LabUser)user).GetPassword()).SecurePassword;

            var msalPublicClient = new PublicClientApplication(ClientId, Authority);

            var authResult =
                await msalPublicClient.AcquireTokenByUsernamePasswordAsync(_scopes, user.Upn, securePassword);
            Assert.IsNotNull(authResult);
            Assert.IsNotNull(authResult.AccessToken);
            Assert.IsNotNull(authResult.IdToken);
            
            // TODO: RE-ENABLE THIS once we have caching and account info in place
            // Assert.AreEqual(user.Upn, authResult.Account.Username);
            // END TODO

            // If test fails with "user needs to consent to the application, do an interactive request" error,
            // Do the following: 
            // 1) Add in code to pull the user's password before creating the SecureString, and put a breakpoint there.
            // string password = ((LabUser)user).GetPassword();
            // 2) Using the MSAL Desktop app, make sure the ClientId matches the one used in integration testing.
            // 3) Do the interactive sign-in with the MSAL Desktop app with the username and password from step 1.
            // 4) After successful log-in, remove the password line you added in with step 1, and run the integration test again.
        }

        [TestMethod]
        [TestCategory("UsernamePasswordIntegrationTests")]
        public async Task AcquireTokenWithFederatedUsernamePasswordAsync()
        {
            var user = _authHelper.GetUser(
                new UserQueryParameters
                {
                    IsMamUser = false,
                    IsMfaUser = false,
                    IsFederatedUser = true
                });

            var securePassword = new NetworkCredential("", user.GetPassword()).SecurePassword;

            var msalPublicClient = new PublicClientApplication(ClientId, Authority);
            var authResult =
                await msalPublicClient.AcquireTokenByUsernamePasswordAsync(_scopes, user.Upn, securePassword);
            Assert.IsNotNull(authResult);
            Assert.IsNotNull(authResult.AccessToken);
            Assert.IsNotNull(authResult.IdToken);
            
            // TODO: RE-ENABLE THIS once we have caching and account info in place
            // Assert.AreEqual(user.Upn, authResult.Account.Username);
            // END TODO

            // If test fails with "user needs to consent to the application, do an interactive request" error,
            // Do the following: 
            // 1) Add in code to pull the user's password before creating the SecureString, and put a breakpoint there.
            // string password = ((LabUser)user).GetPassword();
            // 2) Using the MSAL Desktop app, make sure the ClientId matches the one used in integration testing.
            // 3) Do the interactive sign-in with the MSAL Desktop app with the username and password from step 1.
            // 4) After successful log-in, remove the password line you added in with step 1, and run the integration test again.
        }

        [TestMethod]
        [TestCategory("UsernamePasswordIntegrationTests")]
        public void AcquireTokenWithManagedUsernameIncorrectPassword()
        {
            var user = _authHelper.GetUser(
                new UserQueryParameters
                {
                    IsMamUser = false,
                    IsMfaUser = false,
                    IsFederatedUser = false
                });

            var incorrectSecurePassword = new SecureString();
            incorrectSecurePassword.AppendChar('x');
            incorrectSecurePassword.MakeReadOnly();

            var msalPublicClient = new PublicClientApplication(ClientId, Authority);

            Task<MsalException> result = Assert.ThrowsExceptionAsync<MsalException>(
                async () => await msalPublicClient.AcquireTokenByUsernamePasswordAsync(
                                _scopes,
                                user.Upn,
                                incorrectSecurePassword));
        }

        [TestMethod]
        [TestCategory("UsernamePasswordIntegrationTests")]
        public void AcquireTokenWithFederatedUsernameIncorrectPassword()
        {
            var user = _authHelper.GetUser(
                new UserQueryParameters
                {
                    IsMamUser = false,
                    IsMfaUser = false,
                    IsFederatedUser = true
                });

            var incorrectSecurePassword = new SecureString();
            incorrectSecurePassword.AppendChar('x');
            incorrectSecurePassword.MakeReadOnly();

            var msalPublicClient = new PublicClientApplication(ClientId, Authority);

            Task<MsalException> result = Assert.ThrowsExceptionAsync<MsalException>(
                async () => await msalPublicClient.AcquireTokenByUsernamePasswordAsync(
                                _scopes,
                                user.Upn,
                                incorrectSecurePassword));
        }
    }
}