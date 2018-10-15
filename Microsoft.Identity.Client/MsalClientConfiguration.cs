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
using Microsoft.Identity.Client.Logging;

namespace Microsoft.Identity.Client
{
    public class MsalClientConfiguration
    {
        private readonly object _lockObj = new object();
        private bool _isPiiLoggingEnabled = false;
        private LogLevel _logLevel = LogLevel.Error;
        private TelemetryReceiver _receiver;

        public LogLevel LogLevel
        {
            get
            {
                lock (_lockObj)
                {
                    return _logLevel;
                }
            }
            set
            {
                lock (_lockObj)
                {
                    _logLevel = value;
                }
            }
        }

        public bool IsPiiLoggingEnabled
        {
            get
            {
                lock (_lockObj)
                {
                    return _isPiiLoggingEnabled;
                }
            }
            set
            {
                lock (_lockObj)
                {
                    _isPiiLoggingEnabled = value;
                }
            }
        }

        public string UserAgent { get; set; } = "Mozilla/5.0 (compatible; MSAL 1.0)";
        public int HttpTimeoutSeconds { get; set; } = 10;
        public event EventHandler<LoggerCallbackEventArgs> LoggerCallback;

        internal void InvokeLoggerCallback(
            object sender,
            LoggerCallbackEventArgs e)
        {
            lock (_lockObj)
            {
                LoggerCallback?.Invoke(sender, e);
            }
        }

        public void SetTelemetryReceiver(TelemetryReceiver receiver)
        {
            lock (_lockObj)
            {
                _receiver = receiver;
            }
        }

        internal void InvokeTelemetryReceiver(List<Dictionary<string, string>> events)
        {
            lock (_lockObj)
            {
                _receiver?.Invoke(events);
            }
        }

        public string DefaultClientId { get; set; }
        public string DefaultAuthority { get; set; }
    }
}