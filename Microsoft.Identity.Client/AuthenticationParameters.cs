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
using System.Linq;
using Microsoft.Identity.Client.Logging;
using Microsoft.Identity.Client.Requests;

namespace Microsoft.Identity.Client
{
    public enum AuthorizationType
    {
        None,
        UsernamePassword,
        WindowsIntegratedAuth,
        AuthCode,
        Interactive,
        Certificate
    }

    public class AuthenticationParameters
    {
        private readonly HashSet<string> _requestedScopes = new HashSet<string>();
        public AuthorizationType AuthorizationType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Authority
        {
            get => AuthorityUri?.ToString();
            set => AuthorityUri = new Authority(value);
        }

        internal Authority AuthorityUri { get; set; }
        internal Guid TelemetryCorrelationId { get; set; }
        internal ILogger Logger { get; set; }
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public IEnumerable<string> RequestedScopes => _requestedScopes.AsEnumerable();

        public AuthenticationParameters Clone()
        {
            return new AuthenticationParameters()
            {
                AuthorizationType = AuthorizationType,
                UserName = UserName,
                Password = Password,
                AuthorityUri = AuthorityUri?.Clone(),
                TelemetryCorrelationId = TelemetryCorrelationId
            };
        }

        internal void AddScope(string scope)
        {
            _requestedScopes.Add(scope);
        }

        public void AddScopes(IEnumerable<string> scopes)
        {
            foreach (string scope in scopes)
            {
                AddScope(scope);
            }
        }
    }
}