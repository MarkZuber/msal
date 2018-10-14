using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Identity.Client.Requests
{
    internal class Jwt
    {
        public Jwt(string raw)
        {

        }

        public string Raw { get; }
        public string Payload { get; }

        public bool IsSigned { get; }
        public bool IsEmpty { get; }
    }
}
