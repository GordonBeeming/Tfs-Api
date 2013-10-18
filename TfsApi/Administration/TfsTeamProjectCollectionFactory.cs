// // <copyright file="TfsTeamProjectCollectionFactory.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the TfsTeamProjectCollectionFactory.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Administration
{
    #region

    using System;
    using System.Net;

    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Framework.Common;

    using TfsApi.Contracts;

    #endregion

    internal static class TfsTeamProjectCollectionFactory
    {
        #region Public Methods and Operators

        public static TfsTeamProjectCollection GetTeamProjectCollection(Uri tfsCollectionUri, ITfsCredentials tfsCredentials)
        {
            TfsTeamProjectCollection result;

            TfsClientCredentials credential = null;
            if (tfsCredentials != null)
            {
                credential = tfsCredentials.GetCredentials();
            }
            else
            {
                if (Defaults.GetDefaultCredentials(tfsCollectionUri) != null)
                {
                    credential = Defaults.GetDefaultCredentials(tfsCollectionUri).GetCredentials();
                }
            }

            if (credential != null)
            {
                result = new TfsTeamProjectCollection(tfsCollectionUri, credential);

                result.Connect(ConnectOptions.IncludeServices);
            }
            else
            {
                result = new TfsTeamProjectCollection(tfsCollectionUri);
            }

            return result;
        }

        #endregion

        internal static void Reset()
        {
            // nothing
        }
    }
}