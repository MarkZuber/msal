using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Identity.Client.Requests
{
    internal class RequestDispatcher : IRequestDispatcher
    {
        /// <inheritdoc />
        public Task<AuthenticationResult> ExecuteInteractiveRequestAsync(MsalInteractiveRequest request)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<AuthenticationResult> ExecuteBackgroundRequestAsync(MsalBackgroundRequest request)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task StartupAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task ShutdownAsync()
        {
            throw new NotImplementedException();
        }
    }
}
