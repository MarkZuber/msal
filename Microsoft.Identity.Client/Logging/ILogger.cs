using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Identity.Client.Logging
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogInfoPii(string piiMessage, string noPiiMessage);
    }
}
