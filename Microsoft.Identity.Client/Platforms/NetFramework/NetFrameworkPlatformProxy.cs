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
using Microsoft.Identity.Client.Browser;
using Microsoft.Identity.Client.Cache;
using Microsoft.Identity.Client.Logging;
using Microsoft.Identity.Client.Platform;

namespace Microsoft.Identity.Client.Platforms.NetFramework
{
    internal class NetFrameworkPlatformProxy : IPlatformProxy
    {
        private readonly IBrowserFactory _browserFactory = new NetFrameworkBrowserFactory();
        private readonly ISystemUtils _systemUtils = new NetFrameworkSystemUtils();
        private readonly ICryptographyUtils _cryptographyUtils = new NetFrameworkCryptographyUtils();

        /// <inheritdoc />
        public ISystemUtils GetSystemUtils()
        {
            return _systemUtils;
        }

        /// <inheritdoc />
        public IStorageManager CreateStorageManager()
        {
            return new NetFrameworkStorageManager();
        }

        /// <inheritdoc />
        public IBrowserFactory CreateBrowserFactory()
        {
            return _browserFactory;
        }

        /// <inheritdoc />
        public ICryptographyUtils GetCryptographyUtils()
        {
            return _cryptographyUtils;
        }

        /// <inheritdoc />
        public ILogger CreateLogger(
            Guid correlationId,
            MsalClientConfiguration msalClientConfiguration)
        {
            return new NetFrameworkLogger(GetSystemUtils(), correlationId, msalClientConfiguration);
        }
    }
}