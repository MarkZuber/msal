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

namespace Microsoft.Identity.Client
{
    public enum LogLevel
    {
        Error,
        Warning,
        Info,
        Verbose
    }

    public delegate void LogCallback(
        LogLevel level,
        string message,
        bool containsPii);

    /// <summary>
    ///     This is the legacy static logger...
    /// </summary>
    public static class Logger
    {
        private static readonly object LockObj = new object();
        private static volatile LogCallback _logCallback;

        public static LogCallback LogCallback
        {
            set
            {
                lock (LockObj)
                {
                    if (_logCallback != null)
                    {
                        throw new ArgumentException(
                            "MSAL logging callback can only be set once per process and" +
                            " should never change once set.");
                    }

                    _logCallback = value;
                }
            }
            internal get { return _logCallback; }
        }

        /// <summary>
        /// Enables you to configure the level of logging you want. The default value is <see cref="LogLevel.Info"/>. Setting it to <see cref="LogLevel.Error"/> will only get errors
        /// Setting it to <see cref="LogLevel.Warning"/> will get errors and warning, etc..
        /// </summary>
        public static LogLevel Level { get; set; } = LogLevel.Info;

        /// <summary>
        /// Flag to enable/disable logging of Personally Identifiable data (PII) data. 
        /// PII logs are never written to default outputs like Console, Logcat or NSLog
        /// Default is set to <c>false</c>, which ensures that your application is compliant with GDPR. You can set
        /// it to <c>true</c> for advanced debugging requiring PII
        /// </summary>
        /// <seealso cref="DefaultLoggingEnabled"/>
        public static bool PiiLoggingEnabled { get; set; } = false;

        /// <summary>
        /// Flag to enable/disable logging to platform defaults. In Desktop/UWP, Event Tracing is used. In iOS, NSLog is used.
        /// In android, logcat is used. The default value is <c>false</c>
        /// </summary>
        public static bool DefaultLoggingEnabled { get; set; } = false;
    }
}