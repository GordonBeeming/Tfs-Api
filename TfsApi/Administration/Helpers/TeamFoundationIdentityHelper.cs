using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsApi.Administration.Helpers
{
    #region

    using System;

    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.Framework.Common;

    using TfsApi.Contracts;

    #endregion

    internal static class TeamFoundationIdentityHelper
    {
        #region Public Methods and Operators

        public static TeamFoundationIdentity GetTeamFoundationIdentity(string fullUserName, IdentitySearchFactor identitySearchFactor, TfsTeamProjectCollection tfsTeamProjectCollection)
        {
            IIdentityManagementService ims = (IIdentityManagementService)tfsTeamProjectCollection.GetService(typeof(IIdentityManagementService));
            return ims.ReadIdentity(identitySearchFactor, fullUserName, MembershipQuery.Direct, ReadIdentityOptions.None);
        }

        public static TeamFoundationIdentity GetTeamFoundationIdentityFromDomainUsername(string domain, string username, TfsTeamProjectCollection tfsTeamProjectCollection)
        {
            return GetTeamFoundationIdentity(domain + "\\" + username, IdentitySearchFactor.AccountName, tfsTeamProjectCollection);
        }

        public static TeamFoundationIdentity GetTeamFoundationIdentity(string fullUserName, TfsTeamProjectCollection tfsTeamProjectCollection)
        {
            return GetTeamFoundationIdentity(fullUserName, IdentitySearchFactor.AccountName, tfsTeamProjectCollection);
        }

        #endregion
    }
}
