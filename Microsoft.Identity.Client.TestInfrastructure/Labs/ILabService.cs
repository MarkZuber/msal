using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Identity.Client.TestInfrastructure.Labs
{
    public interface ILabService
    {
        IEnumerable<ILabUser> GetUsers(UserQueryParameters query);
    }
}
