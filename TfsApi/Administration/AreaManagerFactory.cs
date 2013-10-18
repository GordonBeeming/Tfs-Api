using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsApi.Administration.Contracts;
using TfsApi.Administration.Dto;
using TfsApi.Administration.Workers;
using TfsApi.Contracts;

namespace TfsApi.Administration
{
    public static class AreaManagerFactory
    {
        #region Static Fields

        private static readonly ConcurrentDictionary<ProjectDetail, IAreaManager> AreaManagers = new ConcurrentDictionary<ProjectDetail, IAreaManager>();

        #endregion

        #region Public Methods and Operators

        public static IAreaManager GetManager(ProjectDetail projectDetail, ITfsCredentials tfsCredentials = null)
        {
            IAreaManager result;
            if (AreaManagers.FirstOrDefault(o => o.Key.CollectionUri == projectDetail.CollectionUri && o.Key.ProjectName == projectDetail.ProjectName).Value != null)
            {
                result = AreaManagers.FirstOrDefault(o => o.Key.CollectionUri == projectDetail.CollectionUri && o.Key.ProjectName == projectDetail.ProjectName).Value;
            }
            else
            {
                result = new AreaManager(projectDetail, tfsCredentials);
                AreaManagers.AddOrUpdate(projectDetail, result, (key, oldValue) => result);
            }

            return result;
        }

        #endregion

        internal static void Reset()
        {
            AreaManagers.Clear();
        }
    }
}
