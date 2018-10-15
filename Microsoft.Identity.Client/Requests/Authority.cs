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
using System.Globalization;
using Microsoft.Identity.Client.Core;

namespace Microsoft.Identity.Client.Requests
{
    internal class Authority
    {
        private readonly Uri _authorityUri;
        private readonly string _environment;
        private readonly string _realm;

        public Authority(string authorityUri)
        {
            _authorityUri = new Uri(authorityUri);
            _environment = _authorityUri.Host;

            // TODO: use the same regex as MSAL C++ to split the parts out?
            _realm = GetFirstPathSegment(authorityUri);
        }

        public Authority Clone()
        {
            return new Authority(_authorityUri.ToString());
        }

        public Uri GetUserRealmEndpoint(string username)
        {
            return new Uri(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "https://{0}/common/UserRealm/{1}?api-version=1.0",
                    _environment,
                    EncodingUtils.UrlEncode(username)));
        }

        public Uri GetTokenEndpoint()
        {
            return new Uri(
                string.Format(CultureInfo.InvariantCulture, "https://{0}/{1}/oauth2/v2.0/token", _environment, _realm));
        }

        private static string GetFirstPathSegment(string authority)
        {
            return new Uri(authority).Segments[1].TrimEnd('/');
        }
    }
}