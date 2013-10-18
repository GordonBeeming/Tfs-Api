using TfsApi.Administration.Contracts;
namespace TfsApi.Tests
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    using TfsApi.Administration;
    using TfsApi.Administration.Contracts;
    using TfsApi.Administration.Dto;
    using TfsApi.Administration.Tests;

    #endregion

    // ReSharper disable InconsistentNaming
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TeamManagerTests
    {
        [TestMethod]
        public void ctor_TeamManager()
        {
            ProjectDetail projectDetail = this.CreateProjectDetail();

            var obj = TeamManagerFactory.GetManager(projectDetail);
        }

        [TestMethod]
        public void ListTeams_WithDefaults_ShouldReturnListWithCountGreaterThan0()
        {
            ProjectDetail projectDetail = this.CreateProjectDetail();

            var obj = TeamManagerFactory.GetManager(projectDetail);

            int expectedCount = obj.ListTeams().Count;

            Assert.IsTrue(expectedCount > 0);
        }

        [TestMethod]
        public void TeamExists_WithValidTeamName_ShouldReturnTrue()
        {
            ProjectDetail projectDetail = this.CreateProjectDetail();

            var obj = TeamManagerFactory.GetManager(projectDetail);
            string expectedTeamName = GetRandomGuid() + " Team";

            obj.AddTeam(expectedTeamName);

            //Assert.IsTrue(obj.ListTeams().Where(o => o.TeamName == TestConstants.TfsTeamProjectName).First().Teams.Exists(o => o.TeamName == expectedTeamName));
            var actual = obj.TeamExists(expectedTeamName);
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void AddTeam_WithDefaults_ShouldReturnListWithNewTeamInList()
        {
            ProjectDetail projectDetail = this.CreateProjectDetail();

            var obj = TeamManagerFactory.GetManager(projectDetail);
            string expectedTeamName = GetRandomGuid() + " Team";

            obj.AddTeam(expectedTeamName);

            //Assert.IsTrue(obj.ListTeams().Where(o => o.TeamName == TestConstants.TfsTeamProjectName).First().Teams.Exists(o => o.TeamName == expectedTeamName));
            var currentTeams = obj.ListTeams();
            Assert.IsTrue(currentTeams.Exists(o => o.TeamName == expectedTeamName));
        }

        [TestMethod]
        public void DeleteTeam_WithDefaults_ShouldReturnListWithDeletedTeamNotInList()
        {
            ProjectDetail projectDetail = this.CreateProjectDetail();

            var obj = TeamManagerFactory.GetManager(projectDetail);
            string expectedTeamName = GetRandomGuid() + " Team";

            // Add Team
            obj.AddTeam(expectedTeamName);
            var currentTeams = obj.ListTeams();
            Assert.IsTrue(currentTeams.Exists(o => o.TeamName == expectedTeamName));
            
            // Delete Team
            obj.DeleteTeam(expectedTeamName);


            //Check for team deleted

        }

        private ProjectDetail CreateProjectDetail()
        {
            return new ProjectDetail
            {
                CollectionUri = TestConstants.TfsCollectionUri,
                ProjectName = TestConstants.TfsTeamProjectName
            };
        }

        private static string GetRandomGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }

    // ReSharper restore InconsistentNaming
}