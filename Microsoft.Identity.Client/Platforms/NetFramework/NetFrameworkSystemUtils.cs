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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Identity.Client.Platform;

namespace Microsoft.Identity.Client.Platforms.NetFramework
{
    internal class NetFrameworkSystemUtils : ISystemUtils
    {
        private readonly Lazy<string> _clientSku = new Lazy<string>(() => "client_sku");

        // TODO: do the lazy init for all of these values since these will remain constant
        // after first initialization.
        private readonly Lazy<string> _operatingSystem =
            new Lazy<string>(() => Environment.OSVersion.Platform.ToString());

        private readonly Lazy<string> _osVersion = new Lazy<string>(() => Environment.OSVersion.Version.ToString());

        private readonly Lazy<string> _productVersion =
            new Lazy<string>(() => "1.2.3.4"); // TODO: get product version resource

        /// <inheritdoc />
        public string GetCurrentUsername()
        {
            const int NameUserPrincipal = 8;
            uint userNameSize = 0;
            NetFrameworkNativeMethods.GetUserNameEx(NameUserPrincipal, null, ref userNameSize);
            if (userNameSize == 0)
            {
                // todo: exception
                throw new Win32Exception(Marshal.GetLastWin32Error());
                //throw CoreExceptionFactory.Instance.GetClientException(
                //    CoreErrorCodes.GetUserNameFailed,
                //    CoreErrorMessages.GetUserNameFailed,
                //    new Win32Exception(Marshal.GetLastWin32Error()));
            }

            var sb = new StringBuilder((int)userNameSize);
            if (!NetFrameworkNativeMethods.GetUserNameEx(NameUserPrincipal, sb, ref userNameSize))
            {
                // todo: exception
                throw new Win32Exception(Marshal.GetLastWin32Error());
                //throw CoreExceptionFactory.Instance.GetClientException(
                //    CoreErrorCodes.GetUserNameFailed,
                //    CoreErrorMessages.GetUserNameFailed,
                //    new Win32Exception(Marshal.GetLastWin32Error()));
            }

            return sb.ToString();
        }

        /// <inheritdoc />
        public string GetClientSku()
        {
            return _clientSku.Value;
        }

        /// <inheritdoc />
        public string GetOsVersion()
        {
            return _osVersion.Value;
        }

        /// <inheritdoc />
        public string GetProductVersion()
        {
            return _productVersion.Value;
        }

        /// <inheritdoc />
        public string GetOperatingSystem()
        {
            return _operatingSystem.Value;
        }
    }
}