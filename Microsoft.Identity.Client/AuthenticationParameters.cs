using System;
using System.Collections.Generic;
using System.Text;

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

    public class AuthenticationParameters : ICloneable
    {
        /// <inheritdoc />
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public AuthorizationType AuthorizationType { get; set; }
        public string Username { get; set; }
    }
}
