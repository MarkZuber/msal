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
using System.Text;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Logging;
using Microsoft.Identity.Client.Platform;

namespace Microsoft.Identity.Client.Platforms.NetFramework
{
    internal class NetFrameworkLogger : ILogger
    {
        private readonly ISystemUtils _systemUtils;
        private readonly ITimeService _timeService;
        private readonly MsalClientConfiguration _msalClientConfiguration;

        public NetFrameworkLogger(
            ISystemUtils systemUtils,
            Guid correlationId,
            MsalClientConfiguration msalClientConfiguration,
            ITimeService timeService = null)
        {
            _systemUtils = systemUtils;
            CorrelationId = correlationId;
            _msalClientConfiguration = msalClientConfiguration ??
                                       throw new ArgumentNullException(nameof(msalClientConfiguration));
            _timeService = timeService ?? new TimeService();
        }

        /// <inheritdoc />
        public Guid CorrelationId { get; set; }

        /// <inheritdoc />
        public bool PiiLoggingEnabled => _msalClientConfiguration.IsPiiLoggingEnabled;

        /// <inheritdoc />
        public LogLevel Level => _msalClientConfiguration.LogLevel;

        /// <inheritdoc />
        public void Error(string messageScrubbed)
        {
            Log(LogLevel.Error, string.Empty, messageScrubbed);
        }

        /// <inheritdoc />
        public void ErrorPii(
            string messageWithPii,
            string messageScrubbed)
        {
            Log(LogLevel.Error, messageWithPii, messageScrubbed);
        }

        /// <inheritdoc />
        public void ErrorPii(Exception exWithPii)
        {
            Log(LogLevel.Error, exWithPii.ToString(), GetPiiScrubbedExceptionDetails(exWithPii));
        }

        /// <inheritdoc />
        public void ErrorPiiWithPrefix(
            Exception exWithPii,
            string prefix)
        {
            Log(LogLevel.Error, prefix + exWithPii.ToString(), prefix + GetPiiScrubbedExceptionDetails(exWithPii));
        }

        /// <inheritdoc />
        public void Warning(string messageScrubbed)
        {
            Log(LogLevel.Warning, string.Empty, messageScrubbed);
        }

        /// <inheritdoc />
        public void WarningPii(
            string messageWithPii,
            string messageScrubbed)
        {
            Log(LogLevel.Warning, messageWithPii, messageScrubbed);
        }

        /// <inheritdoc />
        public void WarningPii(Exception exWithPii)
        {
            Log(LogLevel.Warning, exWithPii.ToString(), GetPiiScrubbedExceptionDetails(exWithPii));
        }

        /// <inheritdoc />
        public void WarningPiiWithPrefix(
            Exception exWithPii,
            string prefix)
        {
            Log(LogLevel.Warning, prefix + exWithPii.ToString(), prefix + GetPiiScrubbedExceptionDetails(exWithPii));
        }

        /// <inheritdoc />
        public void Info(string messageScrubbed)
        {
            Log(LogLevel.Info, string.Empty, messageScrubbed);
        }

        /// <inheritdoc />
        public void InfoPii(
            string messageWithPii,
            string messageScrubbed)
        {
            Log(LogLevel.Info, messageWithPii, messageScrubbed);
        }

        /// <inheritdoc />
        public void InfoPii(Exception exWithPii)
        {
            Log(LogLevel.Info, exWithPii.ToString(), GetPiiScrubbedExceptionDetails(exWithPii));
        }

        /// <inheritdoc />
        public void InfoPiiWithPrefix(
            Exception exWithPii,
            string prefix)
        {
            Log(LogLevel.Info, prefix + exWithPii.ToString(), prefix + GetPiiScrubbedExceptionDetails(exWithPii));
        }

        /// <inheritdoc />
        public void Verbose(string messageScrubbed)
        {
            Log(LogLevel.Verbose, string.Empty, messageScrubbed);
        }

        /// <inheritdoc />
        public void VerbosePii(
            string messageWithPii,
            string messageScrubbed)
        {
            Log(LogLevel.Verbose, messageWithPii, messageScrubbed);
        }

        private string GetPiiScrubbedExceptionDetails(Exception exWithPii)
        {
            var sb = new StringBuilder();
            if (exWithPii != null)
            {
                sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "Exception type: {0}", exWithPii.GetType()));

                // TODO: IMPLEMENT AS EXCEPTIONS COME ON LINE
                //if (exWithPii is MsalException)
                //{
                //    MsalException msalException = ex as MsalException;
                //    sb.AppendLine(String.Format(
                //        CultureInfo.CurrentCulture,
                //        ", ErrorCode: {0}",
                //        msalException.ErrorCode));
                //}

                //if (exWithPii is MsalServiceException)
                //{
                //    MsalServiceException msalServiceException = ex as MsalServiceException;
                //    sb.AppendLine(String.Format(CultureInfo.CurrentCulture, ", StatusCode: {0}",
                //        msalServiceException.StatusCode));
                //}

                if (exWithPii.InnerException != null)
                {
                    sb.AppendLine("---> Inner Exception Details");
                    sb.AppendLine(GetPiiScrubbedExceptionDetails(exWithPii.InnerException));
                    sb.AppendLine("=== End of inner exception stack trace ===");
                }

                if (exWithPii.StackTrace != null)
                {
                    sb.Append(Environment.NewLine + exWithPii.StackTrace);
                }
            }

            return sb.ToString();
        }

        private void Log(
            LogLevel level,
            string messageWithPii,
            string messageScrubbed)
        {
            if (level > Level)
            {
                return;
            }

            // format log message;
            string correlationId = CorrelationId.Equals(Guid.Empty) ? string.Empty : " - " + CorrelationId;

            bool messageWithPiiExists = !string.IsNullOrWhiteSpace(messageWithPii);
            // If we have a message with PII, and PII logging is enabled, use the PII message, else use the scrubbed message.
            bool isLoggingPii = messageWithPiiExists && PiiLoggingEnabled;
            string messageToLog = isLoggingPii ? messageWithPii : messageScrubbed;

            string log = string.Format(
                CultureInfo.InvariantCulture,
                "{0} MSAL {1} {2} {3} [{4}{5}] {6}",
                isLoggingPii ? "(True)" : "(False)",
                _systemUtils.GetProductVersion(),
                _systemUtils.GetClientSku(),  // todo: ensure this is the same as MSAL today...
                _systemUtils.GetOperatingSystem(),
                _timeService.GetUtcNow(),
                correlationId,
                messageToLog);

            _msalClientConfiguration.InvokeLoggerCallback(this, new LoggerCallbackEventArgs(isLoggingPii, level, log));
        }
    }
}