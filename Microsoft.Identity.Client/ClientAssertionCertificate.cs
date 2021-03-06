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
using System.Security.Cryptography.X509Certificates;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Platform;

namespace Microsoft.Identity.Client
{
    /// <summary>
    ///     Certificate for a client assertion. This class is used in one of the constructors of
    ///     <see cref="ClientCredential" />. ClientCredential
    ///     is itself used in the constructor of <see cref="ConfidentialClientApplication" /> to pass to Azure AD a shared
    ///     secret (registered in the
    ///     Azure AD application)
    /// </summary>
    /// <seealso cref="ClientCredential.ClientCredential(ClientAssertionCertificate)" />
    /// for the constructor of
    /// <seealso cref="ClientCredential" />
    /// with a certificate, and
    /// <seealso
    ///     cref="ConfidentialClientApplication.ConfidentialClientApplication(string, string, ClientCredential, TokenCache, TokenCache)" />
    /// <remarks>
    ///     To understand the difference between public client applications and confidential client applications, see
    ///     https://aka.ms/msal-net-client-applications
    /// </remarks>
    public sealed class ClientAssertionCertificate
    {
        /// <summary>
        ///     Constructor to create certificate information used in
        ///     <see cref="ClientCredential.ClientCredential(ClientAssertionCertificate)" />
        ///     to instantiate a <see cref="ClientCredential" /> used in the constructors of
        ///     <see cref="ConfidentialClientApplication" />
        /// </summary>
        /// <param name="certificate">
        ///     The X509 certificate used as credentials to prove the identity of the application to Azure
        ///     AD.
        /// </param>
        public ClientAssertionCertificate(X509Certificate2 certificate)
        {
            Certificate = certificate ?? throw new ArgumentNullException(nameof(certificate));

#if DESKTOP
            if (certificate.PublicKey.Key.KeySize < MinKeySizeInBits)
            {
                // todo: exception
                throw new ArgumentOutOfRangeException(nameof(certificate));
                //throw new ArgumentOutOfRangeException(nameof(certificate),
                //    string.Format(CultureInfo.InvariantCulture, MsalErrorMessage.CertificateKeySizeTooSmallTemplate,
                //        MinKeySizeInBits));
            }
#endif
        }

        /// <summary>
        ///     Gets minimum X509 certificate key size in bits
        /// </summary>
        public static int MinKeySizeInBits => 2048;

        /// <summary>
        ///     Gets the X509 certificate used as credentials to prove the identity of the application to Azure AD.
        /// </summary>
        public X509Certificate2 Certificate { get; }

        internal string Thumbprint => EncodingUtils.Encode(Certificate.GetCertHash());

        internal byte[] Sign(string message)
        {
            return PlatformProxyFactory.GetPlatformProxy().GetCryptographyUtils()
                                       .SignWithCertificate(message, Certificate);
        }
    }
}