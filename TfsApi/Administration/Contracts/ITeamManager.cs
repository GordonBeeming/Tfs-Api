using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TfsApi.Administration.Contracts
{
    public interface ITeamManager
    {
        void AddMemberToTeam(string teamName, string memberName);
        void AddMemberToTeam(string teamName, IdentityDescriptor descriptor);
        TfsApi.Administration.Contracts.ITfsTeam AddTeam(string teamName, string description = null, System.Collections.Generic.IDictionary<string, object> properties = null);
        System.Collections.Generic.IEnumerable<Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity> GetAdministrators(Microsoft.TeamFoundation.Client.TeamFoundationTeam teamFoundationTeam);
        System.Collections.Generic.IEnumerable<Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity> GetAdministrators(string teamName);
        System.Collections.Generic.IEnumerable<Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity> GetAdministrators(TfsApi.Administration.Contracts.ITfsTeam tfsTeam);
        System.Collections.Generic.IEnumerable<Microsoft.TeamFoundation.Server.Identity> GetMembers();
        TfsApi.Administration.Contracts.ITfsTeam GetTfsTeam(string teamName);
        System.Collections.Generic.List<TfsApi.Administration.Contracts.ITfsTeam> ListTeams(bool ignoreCache = true);

        void DeleteTeam(string expectedTeamName);

        bool TeamExists(string expectedTeamName);
    }
}
