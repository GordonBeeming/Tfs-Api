// // <copyright file="TeamProjectFactory.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the TeamProjectFactory.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Administration
{
    #region

    using System;
    using System.Collections.Concurrent;

    using TfsApi.Administration.Contracts;
    using TfsApi.Contracts;

    #endregion

    public static class TeamProjectFactory
    {
        #region Static Fields

        private static readonly ConcurrentDictionary<Uri, ITeamProjects> TeamProjectManagers = new ConcurrentDictionary<Uri, ITeamProjects>();

        #endregion

        #region Public Methods and Operators

        public static ITeamProjects CreateTeamProjectMananger(Uri tfsCollectionUri, ITfsCredentials tfsCredentials = null)
        {
            ITeamProjects result;
            if (TeamProjectManagers.ContainsKey(tfsCollectionUri))
            {
                result = TeamProjectManagers[tfsCollectionUri];
            }
            else
            {
                result = new TeamProjects(tfsCollectionUri, tfsCredentials);
                TeamProjectManagers.AddOrUpdate(tfsCollectionUri, result, (key, oldValue) => result);
            }

            return result;
        }

        #endregion

        internal static void Reset()
        {
            TeamProjectManagers.Clear();
        }
    }
}