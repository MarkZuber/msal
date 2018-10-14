using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Identity.Client.Requests;

namespace Microsoft.Identity.Client
{
    public class AuthenticationResult
    {
        private AuthenticationResult(Account account, IdToken idToken, string accessToken, DateTime expiresOn, HashSet<string> scopes)
        {
            Account = account;
            IdToken = idToken == null ? string.Empty : idToken.Payload;
            AccessToken = accessToken;
            ExpiresOn = expiresOn;
            Scopes = scopes;
        }

        private AuthenticationResult()
        {
            IsCanceled = true;
        }

        private AuthenticationResult(int errorCode)
        {
            ErrorCode = errorCode;
            IsError = true;
        }

        internal static AuthenticationResult Create(TokenResponse response, Account account)
        {
            return new AuthenticationResult(
                account,
                response.IdToken,
                response.AccessToken,
                response.ExpiresOn,
                response.Scopes);
        }

        internal static AuthenticationResult CreateCanceled()
        {
            return new AuthenticationResult();
        }

        internal static AuthenticationResult CreateWithError(int errorCode)
        {
            return new AuthenticationResult(errorCode);
        }

        public bool IsCanceled { get; }
        public bool IsError { get; }
        public int ErrorCode { get; }

        public Account Account { get; }

        public string IdToken { get; }
        public string AccessToken { get; }
        public DateTime ExpiresOn { get; }
        public IEnumerable<string> Scopes { get; }
        public IEnumerable<string> DeclinedScopes => throw new NotImplementedException();
    }
}
