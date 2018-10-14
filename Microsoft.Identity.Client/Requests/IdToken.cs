using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Identity.Client.Requests
{
    internal class IdToken : Jwt
    {
        public IdToken(string raw) : base(raw)
        {

        }

        public string UserName { get; }
        public string GivenName {get;}
        public string FamilyName { get; }
        public string MiddleName { get; }
        public string Name { get; }
        public string Oid { get; }
        public string TenantId { get; }
        public string Subject { get; }
        public string Upn { get; }
        public string Email { get; }
    }
}
