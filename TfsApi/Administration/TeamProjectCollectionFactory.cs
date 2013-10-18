// // <copyright file="TeamProjectCollectionFactory.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the TeamProjectCollectionFactory.cs type.
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

    public static class TeamProjectCollectionFactory
    {
        #region Static Fields

        private static readonly ConcurrentDictionary<Uri, ITeamProjectCollections> TeamProjectCollectionManagers = new ConcurrentDictionary<Uri, ITeamProjectCollections>();

        #endregion

        #region Public Methods and Operators

        public static ITeamProjectCollections CreateTeamProjectCollectionMananger(Uri tfsUri, ITfsCredentials tfsCredentials = null)
        {
            ITeamProjectCollections result;
            if (TeamProjectCollectionManagers.ContainsKey(tfsUri))
            {
                result = TeamProjectCollectionManagers[tfsUri];
            }
            else
            {
                result = new TeamProjectCollections(tfsUri, tfsCredentials);
                TeamProjectCollectionManagers.AddOrUpdate(tfsUri, result, (key, oldValue) => result);
            }

            return result;
        }

        #endregion

        internal static void Reset()
        {
            TeamProjectCollectionManagers.Clear();
        }
    }
}