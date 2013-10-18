namespace TfsApi.Administration
{
    #region

    using System.Collections.Concurrent;
    using System.Linq;

    using TfsApi.Administration.Contracts;
    using TfsApi.Administration.Dto;
    using TfsApi.Administration.Workers;
    using TfsApi.Contracts;

    #endregion

    public static class IterationManagerFactory
    {
        #region Static Fields

        private static readonly ConcurrentDictionary<ProjectDetail, IIterationManager> IterationManagers = new ConcurrentDictionary<ProjectDetail, IIterationManager>();

        #endregion

        #region Public Methods and Operators

        public static IIterationManager GetManager(ProjectDetail projectDetail, ITfsCredentials tfsCredentials = null)
        {
            IIterationManager result;
            if (IterationManagers.FirstOrDefault(o => o.Key.CollectionUri == projectDetail.CollectionUri && o.Key.ProjectName == projectDetail.ProjectName).Value != null)
            {
                result = IterationManagers.FirstOrDefault(o => o.Key.CollectionUri == projectDetail.CollectionUri && o.Key.ProjectName == projectDetail.ProjectName).Value;
            }
            else
            {
                result = new IterationManager(projectDetail, tfsCredentials);
                IterationManagers.AddOrUpdate(projectDetail, result, (key, oldValue) => result);
            }

            return result;
        }

        #endregion

        internal static void Reset()
        {
            IterationManagers.Clear();
        }
    }
}