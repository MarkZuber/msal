using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Identity.Client.Requests
{
    internal enum CredentialType
    {
        OAuth2AccessToken,
        OAuth2RefreshToken,
        OidcIdToken
    }

    internal class Credential
    {
        public static Credential CreateEmptyCredential()
        {
            return new Credential();
        }

        public static Credential CreateCredential(
            string homeAccountId,
            string environment,
            string realm,
            CredentialType credentialType,
            string clientId,
            string target,
            Int64 cachedAt,
            Int64 expiresOn,
            Int64 extendedExpiresOn,
            string secret,
            string additionalFieldsJson)
        {
            return new Credential();
        }

        public string HomeAccountId { get; set; }
        public string Environment { get; set; }
        public string Secret { get; set; }
        public DateTime ExpiresOn { get; set; }
        public DateTime ExtendedExpiresOn { get; set; }
        public string Target { get; set; }
    }
}
