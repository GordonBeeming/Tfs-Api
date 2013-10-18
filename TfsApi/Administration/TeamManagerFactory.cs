using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsApi.Administration.Dto;
using TfsApi.Administration.Contracts;
using TfsApi.Contracts;
using TfsApi.Administration.Workers;

namespace TfsApi.Administration
{
    public static class TeamManagerFactory
    {
        #region Static Fields

        private static readonly ConcurrentDictionary<ProjectDetail, ITeamManager> TeamManagers = new ConcurrentDictionary<ProjectDetail, ITeamManager>();

        #endregion

        #region Public Methods and Operators

        public static ITeamManager GetManager(ProjectDetail projectDetail, ITfsCredentials tfsCredentials = null)
        {
            ITeamManager result;
            if (TeamManagers.FirstOrDefault(o => o.Key.CollectionUri == projectDetail.CollectionUri && o.Key.ProjectName == projectDetail.ProjectName).Value != null)
            {
                result = TeamManagers.FirstOrDefault(o => o.Key.CollectionUri == projectDetail.CollectionUri && o.Key.ProjectName == projectDetail.ProjectName).Value;
            }
            else
            {
                result = new TeamManager(projectDetail, tfsCredentials);
                TeamManagers.AddOrUpdate(projectDetail, result, (key, oldValue) => result);
            }

            return result;
        }

        #endregion

        internal static void Reset()
        {
            TeamManagers.Clear();
        }
    }
}
