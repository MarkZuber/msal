//------------------------------------------------------------------------------
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
//------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Identity.Client
{
#if !DESKTOP && !NET_CORE
#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
#endif
    /// <summary>
    /// Class to be used to acquire tokens in desktop or mobile applications (Desktop / UWP / Xamarin.iOS / Xamarin.Android).
    /// public client applications are not trusted to safely keep application secrets, and therefore they only access Web APIs in the name of the user only
    /// (they only support public client flows). For details see https://aka.ms/msal-net-client-applications
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>Contrary to <see cref="Microsoft.Identity.Client.ConfidentialClientApplication"/>, public clients are unable to hold configuration time secrets,
    /// and as a result have no client secret</description></item>
    /// <item><description>the redirect URL is pre-proposed by the library. It does not need to be passed in the constructor</description></item>
    /// <item><description>.NET Core does not support UI, and therefore this platform does not provide the interactive token acquisition methods</description></item>
    /// </list>
    /// </remarks>
    public sealed partial class PublicClientApplication : ClientApplicationBase, IPublicClientApplication
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
    {
        ///// <summary>
        ///// Constructor of the application. It will use https://login.microsoftonline.com/common as the default authority.
        ///// </summary>
        ///// <param name="clientId">Client ID (also known as App ID) of the application as registered in the
        ///// application registration portal (https://aka.ms/msal-net-register-app)/. REQUIRED</param>
        //public PublicClientApplication(string clientId) : this(clientId, DefaultAuthority)
        //{
        //}

        ///// <summary>
        ///// Constructor of the application.
        ///// </summary>
        ///// <param name="clientId">Client ID (also named Application ID) of the application as registered in the
        ///// application registration portal (https://aka.ms/msal-net-register-app)/. REQUIRED</param>
        ///// <param name="authority">Authority of the security token service (STS) from which MSAL.NET will acquire the tokens.
        ///// Usual authorities are:
        ///// <list type="bullet">
        ///// <item><description><c>https://login.microsoftonline.com/tenant/</c>, where <c>tenant</c> is the tenant ID of the Azure AD tenant
        ///// or a domain associated with this Azure AD tenant, in order to sign-in user of a specific organization only</description></item>
        ///// <item><description><c>https://login.microsoftonline.com/common/</c> to signing users with any work and school accounts or Microsoft personal account</description></item>
        ///// <item><description><c>https://login.microsoftonline.com/organizations/</c> to signing users with any work and school accounts</description></item>
        ///// <item><description><c>https://login.microsoftonline.com/consumers/</c> to signing users with only personal Microsoft account (live)</description></item>
        ///// </list>
        ///// Note that this setting needs to be consistent with what is declared in the application registration portal
        ///// </param>
        //public PublicClientApplication(string clientId, string authority)
        //    : this(null, clientId, authority)
        //{
        //    UserTokenCache = new TokenCache()
        //    {
        //        ClientId = clientId
        //    };
        //}

        // netcoreapp does not support UI at the moment and all the Acquire* methods use UI;
#if !NET_CORE

#if iOS
        private string keychainSecurityGroup;

        /// <summary>
        /// Xamarin iOS specific property enabling the application to share the token cache with other applications sharing the same keychain security group.
        /// If you provide this key, you MUST add the capability to your Application Entitlement.
        /// For more details, please see https://aka.ms/msal-net-sharing-cache-on-ios
        /// </summary>
        /// <remarks>This API may change in future release.</remarks>
        public string KeychainSecurityGroup
        {
            get
            {
                return keychainSecurityGroup;
            }
            set
            {
                keychainSecurityGroup = value;
                UserTokenCache.tokenCacheAccessor.SetKeychainSecurityGroup(value);
                UserTokenCache.legacyCachePersistence.SetKeychainSecurityGroup(value);
            }
        }
#endif

#if WINDOWS_APP
        /// <summary>
        /// Flag to enable authentication with the user currently logged-in in Windows.
        /// When set to true, the application will try to connect to the corporate network using windows integrated authentication.
        /// </summary>
        public bool UseCorporateNetwork { get; set; }
#endif

#if !ANDROID
        /// <summary>
        /// Interactive request to acquire token for the specified scopes. The user is required to select an account
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        /// <remarks>The user will be signed-in interactively if needed,
        /// and will consent to scopes and do multi-factor authentication if such a policy was enabled in the Azure AD tenant.</remarks>
        public Task<AuthenticationResult> AcquireTokenAsync(IEnumerable<string> scopes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interactive request to acquire token for the specified scopes. The user will need to sign-in but an account will be proposed
        /// based on the <paramref name="loginHint"/>
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="loginHint">Identifier of the user. Generally in UserPrincipalName (UPN) format, e.g. <c>john.doe@contoso.com</c></param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        public Task<AuthenticationResult> AcquireTokenAsync(IEnumerable<string> scopes, string loginHint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interactive request to acquire token for the specified scopes. The user will need to sign-in but an account will be proposed
        /// based on the provided <paramref name="account"/>
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="account">Account to use for the interactive token acquisition. See <see cref="IAccount"/> for ways to get an account</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        public Task<AuthenticationResult> AcquireTokenAsync(
            IEnumerable<string> scopes,
            IAccount account)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interactive request to acquire token for a login with control of the UI behavior and possiblity of passing extra query parameters like additional claims
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="loginHint">Identifier of the user. Generally in UserPrincipalName (UPN) format, e.g. <c>john.doe@contoso.com</c></param>
        /// <param name="behavior">Designed interactive experience for the user.</param>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority.
        /// This is expected to be a string of segments of the form <c>key=value</c> separated by an ampersand character.
        /// The parameter can be null.</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        public Task<AuthenticationResult> AcquireTokenAsync(IEnumerable<string> scopes, string loginHint,
            UIBehavior behavior, string extraQueryParameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interactive request to acquire token for an account with control of the UI behavior and possiblity of passing extra query parameters like additional claims
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="account">Account to use for the interactive token acquisition. See <see cref="IAccount"/> for ways to get an account</param>
        /// <param name="behavior">Designed interactive experience for the user.</param>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority.
        /// This is expected to be a string of segments of the form <c>key=value</c> separated by an ampersand character.
        /// The parameter can be null.</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        public Task<AuthenticationResult> AcquireTokenAsync(IEnumerable<string> scopes, IAccount account,
            UIBehavior behavior, string extraQueryParameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interactive request to acquire token for a given login, with the possibility of controlling the user experience, passing extra query
        /// parameters, providing extra scopes that the user can pre-consent to, and overriding the authority pre-configured in the application
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="loginHint">Identifier of the user. Generally in UserPrincipalName (UPN) format, e.g. <c>john.doe@contoso.com</c></param>
        /// <param name="behavior">Designed interactive experience for the user.</param>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority.
        /// This is expected to be a string of segments of the form <c>key=value</c> separated by an ampersand character.
        /// The parameter can be null.</param>
        /// <param name="extraScopesToConsent">Scopes that you can request the end user to consent upfront, in addition to the scopes for the protected Web API
        /// for which you want to acquire a security token.</param>
        /// <param name="authority">Specific authority for which the token is requested. Passing a different value than configured does not change the configured value</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        public Task<AuthenticationResult> AcquireTokenAsync(IEnumerable<string> scopes, string loginHint,
            UIBehavior behavior, string extraQueryParameters, IEnumerable<string> extraScopesToConsent, string authority)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interactive request to acquire token for a given account, with the possibility of controlling the user experience, passing extra query
        /// parameters, providing extra scopes that the user can pre-consent to, and overriding the authority pre-configured in the application
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="account">Account to use for the interactive token acquisition. See <see cref="IAccount"/> for ways to get an account</param>
        /// <param name="behavior">Designed interactive experience for the user.</param>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority.
        /// This is expected to be a string of segments of the form <c>key=value</c> separated by an ampersand character.
        /// The parameter can be null.</param>
        /// <param name="extraScopesToConsent">Scopes that you can request the end user to consent upfront, in addition to the scopes for the protected Web API
        /// for which you want to acquire a security token.</param>
        /// <param name="authority">Specific authority for which the token is requested. Passing a different value than configured does not change the configured value</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        public Task<AuthenticationResult> AcquireTokenAsync(IEnumerable<string> scopes, IAccount account,
            UIBehavior behavior, string extraQueryParameters, IEnumerable<string> extraScopesToConsent, string authority)
        {
            throw new NotImplementedException();
        }
#endif

        /// <summary>
        /// Interactive request to acquire token for the specified scopes. The interactive window will be parented to the specified
        /// window. The user will be required to select an account
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="parent">Object containing a reference to the parent window/activity. REQUIRED for Xamarin.Android only.</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        /// <remarks>The user will be signed-in interactively if needed,
        /// and will consent to scopes and do multi-factor authentication if such a policy was enabled in the Azure AD tenant.</remarks>
        public Task<AuthenticationResult> AcquireTokenAsync(IEnumerable<string> scopes, UIParent parent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interactive request to acquire token for the specified scopes. The interactive window will be parented to the specified
        /// window. The user will need to sign-in but an account will be proposed
        /// based on the <paramref name="loginHint"/>
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="loginHint">Identifier of the user. Generally in UserPrincipalName (UPN) format, e.g. <c>john.doe@contoso.com</c></param>
        /// <param name="parent">Object containing a reference to the parent window/activity. REQUIRED for Xamarin.Android only.</param>
        /// <returns>Authentication result containing a token for the requested scopes and login</returns>
        public Task<AuthenticationResult> AcquireTokenAsync(IEnumerable<string> scopes, string loginHint, UIParent parent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interactive request to acquire token for the specified scopes. The user will need to sign-in but an account will be proposed
        /// based on the provided <paramref name="account"/>
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="account">Account to use for the interactive token acquisition. See <see cref="IAccount"/> for ways to get an account</param>
        /// <param name="parent">Object containing a reference to the parent window/activity. REQUIRED for Xamarin.Android only.</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        public Task<AuthenticationResult> AcquireTokenAsync(
            IEnumerable<string> scopes,
            IAccount account, UIParent parent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interactive request to acquire token for a login with control of the UI behavior and possiblity of passing extra query parameters like additional claims
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="loginHint">Identifier of the user. Generally in UserPrincipalName (UPN) format, e.g. <c>john.doe@contoso.com</c></param>
        /// <param name="behavior">Designed interactive experience for the user.</param>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority.
        /// This is expected to be a string of segments of the form <c>key=value</c> separated by an ampersand character.
        /// The parameter can be null.</param>
        /// <param name="parent">Object containing a reference to the parent window/activity. REQUIRED for Xamarin.Android only.</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        public Task<AuthenticationResult> AcquireTokenAsync(IEnumerable<string> scopes, string loginHint,
            UIBehavior behavior, string extraQueryParameters, UIParent parent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interactive request to acquire token for an account with control of the UI behavior and possiblity of passing extra query parameters like additional claims
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="account">Account to use for the interactive token acquisition. See <see cref="IAccount"/> for ways to get an account</param>
        /// <param name="behavior">Designed interactive experience for the user.</param>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority.
        /// This is expected to be a string of segments of the form <c>key=value</c> separated by an ampersand character.
        /// The parameter can be null.</param>
        /// <param name="parent">Object containing a reference to the parent window/activity. REQUIRED for Xamarin.Android only.</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        public Task<AuthenticationResult> AcquireTokenAsync(IEnumerable<string> scopes, IAccount account,
            UIBehavior behavior, string extraQueryParameters, UIParent parent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interactive request to acquire token for a given login, with the possibility of controlling the user experience, passing extra query
        /// parameters, providing extra scopes that the user can pre-consent to, and overriding the authority pre-configured in the application
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="loginHint">Identifier of the user. Generally in UserPrincipalName (UPN) format, e.g. <c>john.doe@contoso.com</c></param>
        /// <param name="behavior">Designed interactive experience for the user.</param>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority.
        /// This is expected to be a string of segments of the form <c>key=value</c> separated by an ampersand character.
        /// The parameter can be null.</param>
        /// <param name="extraScopesToConsent">scopes that you can request the end user to consent upfront, in addition to the scopes for the protected Web API
        /// for which you want to acquire a security token.</param>
        /// <param name="authority">Specific authority for which the token is requested. Passing a different value than configured does not change the configured value</param>
        /// <param name="parent">Object containing a reference to the parent window/activity. REQUIRED for Xamarin.Android only.</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        public Task<AuthenticationResult> AcquireTokenAsync(IEnumerable<string> scopes, string loginHint,
            UIBehavior behavior, string extraQueryParameters, IEnumerable<string> extraScopesToConsent, string authority, UIParent parent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interactive request to acquire token for a given account, with the possibility of controlling the user experience, passing extra query
        /// parameters, providing extra scopes that the user can pre-consent to, and overriding the authority pre-configured in the application
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="account">Account to use for the interactive token acquisition. See <see cref="IAccount"/> for ways to get an account</param>
        /// <param name="behavior">Designed interactive experience for the user.</param>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority.
        /// This is expected to be a string of segments of the form <c>key=value</c> separated by an ampersand character.
        /// The parameter can be null.</param>
        /// <param name="extraScopesToConsent">scopes that you can request the end user to consent upfront, in addition to the scopes for the protected Web API
        /// for which you want to acquire a security token.</param>
        /// <param name="authority">Specific authority for which the token is requested. Passing a different value than configured does not change the configured value</param>
        /// <param name="parent">Object containing a reference to the parent window/activity. REQUIRED for Xamarin.Android only.</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        public Task<AuthenticationResult> AcquireTokenAsync(IEnumerable<string> scopes, IAccount account,
        UIBehavior behavior, string extraQueryParameters, IEnumerable<string> extraScopesToConsent, string authority, UIParent parent)
        {
            throw new NotImplementedException();
        }
    }

    #endif
}