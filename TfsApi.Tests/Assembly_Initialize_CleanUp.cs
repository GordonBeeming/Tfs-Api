// // <copyright file="Assembly_Initialize_CleanUp.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the Assembly_Initialize_CleanUp.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Administration.Tests
{
    #region

    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TfsApi.Administration.Contracts;
    using System;

    #endregion

    [TestClass]
    [ExcludeFromCodeCoverage]
    public sealed class Assembly_Initialize_CleanUp
    {
        #region Public Methods and Operators

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            //ITeamProjects teamProjectCollections = TeamProjectFactory.CreateTeamProjectMananger(TestConstants.TfsCollectionUri, TestConstants.DefaultCredentials);

            //foreach (string projectName in teamProjectCollections.ListTeamProjectNames())
            //{
            //    teamProjectCollections.DeleteTeamProject(projectName, true, false);
            //}

            ITeamManager teamManager = TeamManagerFactory.GetManager(TestConstants.TfsTeamProjectDetail);
            foreach (var team in teamManager.ListTeams())
            {
                if (team.TeamName != TestConstants.TfsTeamProjectName + " Team")
                {
                    teamManager.DeleteTeam(team.TeamName);
                }
            }
        }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            using (ITeamProjectCollections teamProjectCollections = TeamProjectCollectionFactory.CreateTeamProjectCollectionMananger(TestConstants.TfsUri, TestConstants.DefaultCredentials))
            {
                if (!teamProjectCollections.CollectionExists(TestConstants.TfsCollectionName))
                {
                    teamProjectCollections.CreateProjectCollection(TestConstants.TfsCollectionName, TestConstants.TfsCollectionDescription);
                }
            }

            using (ITeamProjects teamProjectCollections = TeamProjectFactory.CreateTeamProjectMananger(TestConstants.TfsCollectionUri, TestConstants.DefaultCredentials))
            {
                if (!teamProjectCollections.TeamProjectExists(TestConstants.TfsTeamProjectName))
                {
                    string processName = string.Empty;
                    IProcessTemplates processTemplates = ProcessTemplateFactory.CreateProcessTemplateMananger(TestConstants.TfsCollectionUri);
                    processName = processTemplates.ListProcessTemplates()[0].Name;
                    if (!teamProjectCollections.CreateTeamProject(TestConstants.TfsTeamProjectName, TestConstants.TfsTeamProjectDescription, processName, false, false))
                    {
                        throw new Exception("Failed to create the default project for TFS API Tests.");
                    }
                }
            }
        }

        #endregion
    }
}