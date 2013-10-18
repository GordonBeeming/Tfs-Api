// // <copyright file="TestConstants.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the TestConstants.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Administration.Tests
{
    #region

    using System;
    using System.Diagnostics.CodeAnalysis;
    using TfsApi.Administration.Contracts;
    using TfsApi.Administration.Dto;
    using TfsApi.Contracts;

    #endregion

    [ExcludeFromCodeCoverage]
    public static class TestConstants
    {
        #region Public Properties

        public static ITfsCredentials DefaultCredentials
        {
            get
            {
                return null;
                //return new BasicCredential("Administrator", "Passw0rd");
            }
        }

        public static string ProcessTemplateDescription
        {
            get
            {
                return "Tfs API Process Template Description";
            }
        }

        public static string ProcessTemplateName
        {
            get
            {
                return "Tfs API Process Template";
            }
        }

        public static string TfsCollectionDescription
        {
            get
            {
                return @"This is the Tfs Api Collection created as part of the Unit Tests Assembly Initialization. 

More Info : http://tfsapi.codeplex.com.";
            }
        }

        public static string TfsCollectionName
        {
            get
            {
                return @"TfsAPI";
            }
        }

        public static Uri TfsCollectionUri
        {
            get
            {
                return new Uri(TfsUri + "/" + TfsCollectionName);
            }
        }

        public static string TfsTeamProjectDescription
        {
            get
            {
                return @"This is the Tfs Api '" + TfsTeamProjectName + @"' Team Project created as part of the Unit Tests Assembly Initialization. 

More Info : http://tfsapi.codeplex.com.";
            }
        }

        public static string TfsTeamProjectName
        {
            get
            {
                return @"Team 0";
            }
        }

        public static Uri TfsTeamProjectUri
        {
            get
            {
                return new Uri(TfsCollectionUri + "/" + TfsTeamProjectName);
            }
        }

        public static Uri TfsUri
        {
            get
            {
                return new Uri("http://DerTfs02");
            }
        }

        public static ProjectDetail TfsTeamProjectDetail
        {
            get
            {
                return new ProjectDetail
                {
                    CollectionUri = TfsCollectionUri,
                    ProjectName = TfsTeamProjectName
                };
            }
        }

        public static ITfsTeam TfsTeam
        {
            get
            {
                ITeamManager teamManager = TeamManagerFactory.GetManager(TfsTeamProjectDetail);
                var tfsTeamName = TfsTeamProjectDetail.ProjectName + " Team";
                return teamManager.GetTfsTeam(tfsTeamName);
            }
        }

        #endregion
    }
}