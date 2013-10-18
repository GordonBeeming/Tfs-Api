using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TfsApi.Administration.Contracts;
using TfsApi.Administration.Dto;
using TfsApi.Administration.Helpers;

namespace TfsApi.Administration.Workers
{
    internal class TeamManager : ITeamManager
    {
        private readonly ICommonStructureService4 commonStructureService;

        /// <summary>
        ///     The project detail.
        /// </summary>
        private readonly ProjectDetail projectDetail;

        private readonly ProjectInfo projectInfo;

        private readonly TfsTeamProjectCollection tfsTeamProjectCollection;

        private readonly TeamSettingsConfigurationService teamSettingsConfigurationService;

        private readonly IGroupSecurityService groupSecurityService;

        private readonly Microsoft.TeamFoundation.Client.TfsTeamService TfsTeamService;

        private List<ITfsTeam> _localTfsTeamCache = null;

        private readonly TfsApi.Contracts.ITfsCredentials tfsCredentials;

        public TeamManager(Dto.ProjectDetail projectDetail, TfsApi.Contracts.ITfsCredentials tfsCredentials)
        {
            this.tfsCredentials = tfsCredentials;

            this.projectDetail = projectDetail;

            this.tfsTeamProjectCollection = TfsApi.Administration.TfsTeamProjectCollectionFactory.GetTeamProjectCollection(this.projectDetail.CollectionUri, this.tfsCredentials);

            this.teamSettingsConfigurationService = this.tfsTeamProjectCollection.GetService<TeamSettingsConfigurationService>();

            this.commonStructureService = (ICommonStructureService4)this.tfsTeamProjectCollection.GetService(typeof(ICommonStructureService4));

            this.projectInfo = this.commonStructureService.GetProjectFromName(this.projectDetail.ProjectName);

            this.groupSecurityService = (IGroupSecurityService)this.tfsTeamProjectCollection.GetService<IGroupSecurityService>();

            this.TfsTeamService = (Microsoft.TeamFoundation.Client.TfsTeamService)this.tfsTeamProjectCollection.GetService(typeof(Microsoft.TeamFoundation.Client.TfsTeamService));
        }

        public List<ITfsTeam> ListTeams(bool ignoreCache = true)
        {
            if (!ignoreCache)
            {
                return _localTfsTeamCache;
            }
            else
            {
                List<ITfsTeam> result = new List<ITfsTeam>();

                foreach (TeamConfiguration teamConfiguration in teamSettingsConfigurationService.GetTeamConfigurationsForUser(new[] { projectInfo.Uri }))
                {
                    ITfsTeam team = CreateTfsTeamFromTeamConfiguration(teamConfiguration);
                    result.Add(team);
                }
                _localTfsTeamCache = result;
                return result;
            }
        }

        public IEnumerable<Identity> GetMembers()
        {
            var groupSid = groupSecurityService.ReadIdentity(SearchFactor.AccountName, "Project Collection Valid Users", QueryMembership.Expanded);

            return groupSecurityService.ReadIdentities(SearchFactor.Sid, groupSid.Members, QueryMembership.None).Where(a => a.Type == IdentityType.WindowsUser || a.Type == IdentityType.WindowsGroup);
        }

        public IEnumerable<TeamFoundationIdentity> GetAdministrators(string teamName)
        {
            return GetAdministrators(TfsTeamService.ReadTeam(this.projectInfo.Uri, teamName, null));
        }

        public IEnumerable<TeamFoundationIdentity> GetAdministrators(ITfsTeam tfsTeam)
        {
            return GetAdministrators(tfsTeam.TeamFoundationTeam);
        }

        public IEnumerable<TeamFoundationIdentity> GetAdministrators(Microsoft.TeamFoundation.Client.TeamFoundationTeam teamFoundationTeam)
        {
            // Get security namespace for the project collection.
            ISecurityService securityService = this.tfsTeamProjectCollection.GetService<ISecurityService>();
            SecurityNamespace securityNamespace = securityService.GetSecurityNamespace(FrameworkSecurity.IdentitiesNamespaceId);

            // Use reflection to retrieve a security token for the team.
            MethodInfo mi = typeof(IdentityHelper).GetMethod("CreateSecurityToken");
            string token = mi.Invoke(null, new object[] { teamFoundationTeam.Identity }) as string;

            // Retrieve an ACL object for all the team members.
            var allMembers = teamFoundationTeam.GetMembers(this.tfsTeamProjectCollection, MembershipQuery.Expanded).Where(m => !m.IsContainer);
            AccessControlList acl = securityNamespace.QueryAccessControlList(token, allMembers.Select(m => m.Descriptor), true);

            // Retrieve the team administrator SIDs by querying the ACL entries.
            var entries = acl.AccessControlEntries;
            var admins = entries.Where(e => (e.Allow & 15) == 15).Select(e => e.Descriptor.Identifier);

            // Finally, retrieve the actual TeamFoundationIdentity objects from the SIDs.
            return allMembers.Where(m => admins.Contains(m.Descriptor.Identifier));
        }

        public void AddMemberToTeam(string teamName, string memberName)
        {
            var memberSid = TeamFoundationIdentityHelper.GetTeamFoundationIdentity(memberName, this.tfsTeamProjectCollection);

            AddMemberToTeam(teamName, memberSid.Descriptor);
        }

        public void AddMemberToTeam(string teamName, IdentityDescriptor descriptor)
        {
            var groupSid = GetGroupIdentityForTeamName(teamName);

            groupSecurityService.AddMemberToApplicationGroup(groupSid.Sid, descriptor.Identifier);
        }

        private Identity GetGroupIdentityForTeamName(string teamName)
        {
            return groupSecurityService.ReadIdentity(SearchFactor.AccountName, "[" + projectDetail.ProjectName + "]\\" + teamName, QueryMembership.Expanded);
        }

        private ITfsTeam CreateTfsTeamFromTeamConfiguration(TeamConfiguration teamConfiguration)
        {
            return new TfsTeam(tfsCredentials)
            {
                IsDefaultTeam = teamConfiguration.IsDefaultTeam,
                ProjectUri = teamConfiguration.ProjectUri,
                TeamId = teamConfiguration.TeamId,
                TeamName = teamConfiguration.TeamName,
                TeamSettings = teamConfiguration.TeamSettings,
                TeamConfiguration = teamConfiguration,
                TeamSettingsConfigurationService = teamSettingsConfigurationService,
                TeamService = this.tfsTeamProjectCollection.GetService<TfsTeamService>(),
                ProjectDetails = projectDetail
            };
        }

        public ITfsTeam GetTfsTeam(string teamName)
        {
            return ListTeams().Where(o => string.Compare(o.TeamName, teamName, true) == 0).FirstOrDefault();
        }

        private bool ConfigurationIsForTeamProjectInScope(TeamConfiguration teamConfiguration)
        {
            return string.Compare(projectDetail.ProjectName, teamConfiguration.TeamName, true) == 0;
        }

        private bool ScopeAtProjectLevelAndCollectionCotainsTeams(List<ITfsTeam> result)
        {
            return !ScopeAtCollectionLevel() && result.Count > 0;
        }

        private bool ScopeAtCollectionLevel()
        {
            return string.IsNullOrEmpty(projectDetail.ProjectName);
        }

        public ITfsTeam AddTeam(string teamName, string description = null, IDictionary<string, object> properties = null)
        {
            TfsTeamService.CreateTeam(projectInfo.Uri, teamName, description, properties);
            AddMemberToTeam(teamName, this.tfsTeamProjectCollection.AuthorizedIdentity.Descriptor);
            ListTeams(true);
            return GetTfsTeam(teamName);
        }


        public void DeleteTeam(string teamName)
        {
            var groupSid = GetGroupIdentityForTeamName(teamName);

            groupSecurityService.DeleteApplicationGroup(groupSid.Sid);
            ListTeams(true);
        }

        public bool TeamExists(string teamName)
        {
            return ListTeams().Exists(o => o.TeamName == teamName);
        }
    }
}
