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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Identity.Client.Core;

namespace Microsoft.Identity.Client.TestInfrastructure.Labs
{
    public class KeyVaultSecretsProvider
    {
        /// <summary>
        ///     Token cache used by the test infrastructure when authenticating against KeyVault
        /// </summary>
        /// <remarks>
        ///     We aren't using the default cache to make sure the tokens used by this
        ///     test infrastructure can't end up in the cache being used by the tests (the UI-less
        ///     Desktop test app runs in the same AppDomain as the infrastructure and uses the
        ///     default cache).
        /// </remarks>
        // private static readonly TokenCache keyVaultTokenCache = new TokenCache();

        private readonly KeyVaultConfiguration _config;
        private readonly KeyVaultClient _keyVaultClient;
        private const string KeyVaultClientId = "ebe49c8f-61de-4357-9194-7a786f6402b4";
        private const string KeyVaultThumbPrint = "440A5BE6C4BE2FF02A0ADBED1AAA43D6CF12E269";

        /// <summary>Initialize the secrets provider with the "keyVault" configuration section.</summary>
        /// <remarks>
        ///     <para>
        ///         Authentication using <see cref="KeyVaultAuthenticationType.ClientCertificate" />
        ///         1. Register Azure AD application of "Web app / API" type.
        ///         To set up certificate based access to the application PowerShell should be used.
        ///         2. Add an access policy entry to target Key Vault instance for this application.
        ///         The "keyVault" configuration section should define:
        ///         "authType": "ClientCertificate"
        ///         "clientId": [client ID]
        ///         "certThumbprint": [certificate thumbprint]
        ///     </para>
        ///     <para>
        ///         Authentication using <see cref="KeyVaultAuthenticationType.UserCredential" />
        ///         1. Register Azure AD application of "Native" type.
        ///         2. Add to 'Required permissions' access to 'Azure Key Vault (AzureKeyVault)' API.
        ///         3. When you run your native client application, it will automatically prompt user to enter Azure AD
        ///         credentials.
        ///         4. To successfully access keys/secrets in the Key Vault, the user must have specific permissions to perform
        ///         those operations.
        ///         This could be achieved by directly adding an access policy entry to target Key Vault instance for this user
        ///         or an access policy entry for an Azure AD security group of which this user is a member of.
        ///         The "keyVault" configuration section should define:
        ///         "authType": "UserCredential"
        ///         "clientId": [client ID]
        ///     </para>
        /// </remarks>
        public KeyVaultSecretsProvider()
        {
            _config = new KeyVaultConfiguration
            {
                AuthType = KeyVaultAuthenticationType.ClientCertificate,
                ClientId = KeyVaultClientId,
                CertThumbprint = KeyVaultThumbPrint
            };
            _keyVaultClient = new KeyVaultClient(AuthenticationCallbackAsync);
        }

        public SecretBundle GetSecret(string secretUrl)
        {
            return _keyVaultClient.GetSecretAsync(secretUrl).GetAwaiter().GetResult();
        }

        private async Task<string> AuthenticationCallbackAsync(
            string authority,
            string resource,
            string scope)
        {
            var msalConfiguration = new MsalClientConfiguration();
            var pca = new PublicClientApplication(msalConfiguration);
            //var authContext = new AuthenticationContext(authority, keyVaultTokenCache);

            var authParameters = new AuthenticationParameters
            {
                Authority = authority,
                ClientId = _config.ClientId,
            };
            authParameters.AddScopes(ScopeUtils.Split(scope));
            authParameters.AddScope("https://vault.azure.net/.default");

            switch (_config.AuthType)
            {
            case KeyVaultAuthenticationType.ClientCertificate:
                var cert = CertificateHelper.FindCertificateByThumbprint(_config.CertThumbprint);
                authParameters.Certificate = cert;
                authParameters.AuthorizationType = AuthorizationType.Certificate;

                // authContext.AcquireTokenAsync(resource, _assertionCert));
                break;
            case KeyVaultAuthenticationType.UserCredential:
                authParameters.AuthorizationType = AuthorizationType.WindowsIntegratedAuth;
                //authResult = await authContext.AcquireTokenAsync(resource, _config.ClientId, new UserCredential());
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }

            var authResult = await pca.AcquireTokenSilentlyAsync(authParameters, CancellationToken.None)
                                      .ConfigureAwait(false);
            return authResult?.AccessToken;
        }
    }
}