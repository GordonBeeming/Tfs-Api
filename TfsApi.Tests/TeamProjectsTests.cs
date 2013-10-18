// // <copyright file="TeamProjectsTests.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the TeamProjectsTests.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Administration.Tests
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TfsApi.Administration.Contracts;

    #endregion

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TeamProjectsTests
    {
        #region Constants

        private const string TestProjectNameBase = "Project Page Tests";

        #endregion

        #region Public Methods and Operators

        [TestCleanup]
        public void ClassCleanup()
        {
            ITeamProjects teamProjects = TeamProjectFactory.CreateTeamProjectMananger(TestConstants.TfsCollectionUri);

            foreach (string projectName in teamProjects.ListTeamProjectNames())
            {
                if (projectName.StartsWith(TestProjectNameBase))
                {
                    teamProjects.DeleteTeamProject(projectName, true, false);
                }
            }
        }

        [TestMethod]
        [Priority(99)]
        public void CreateAndDeleteTeamProject_PassInTestProjectDetails_ProjectListToReturnTheNewProjectInTheListAndThenTheProjectToBeRemovedAndVerifyTheWarnings()
        {
            // arrange
            ITeamProjects teamProjects = TeamProjectFactory.CreateTeamProjectMananger(TestConstants.TfsCollectionUri);
            string projectName = TestProjectNameBase + Guid.NewGuid().ToString("N");

            IProcessTemplates processTemplates = ProcessTemplateFactory.CreateProcessTemplateMananger(TestConstants.TfsCollectionUri);
            string processTemplateName = processTemplates.ListProcessTemplates()[0].Name;

            // act
            teamProjects.CreateTeamProject(projectName, projectName + " Description", processTemplateName, false, false);

            // assert
            bool actual = false;
            foreach (string project in teamProjects.ListTeamProjectNames())
            {
                if (string.Compare(project, projectName, true) == 0)
                {
                    actual = true;
                    break;
                }
            }

            Assert.IsTrue(actual, "Create Failed.");
            List<Exception> exceptions;
            teamProjects.DeleteTeamProject(projectName, out exceptions, true, false);

            bool actualAfter = false;
            foreach (string project in teamProjects.ListTeamProjectNames())
            {
                if (string.Compare(project, projectName, true) == 0)
                {
                    actualAfter = true;
                    break;
                }
            }

            Assert.IsFalse(actualAfter, "Delete Failed.");
        }

        [TestMethod]
        [Priority(50)]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateTeamProject_PassInTestProjectDetailsWithExistingProjectName_ArgumentExceptionThrown()
        {
            // arrange
            ITeamProjects teamProjects = TeamProjectFactory.CreateTeamProjectMananger(TestConstants.TfsCollectionUri);
            string projectName = TestConstants.TfsTeamProjectName;

            IProcessTemplates processTemplates = ProcessTemplateFactory.CreateProcessTemplateMananger(TestConstants.TfsCollectionUri);
            string processTemplateName = processTemplates.ListProcessTemplates()[0].Name;

            // act
            teamProjects.CreateTeamProject(projectName, projectName + " Description", processTemplateName, false, false);

            // assert
            bool actual = false;
            foreach (string project in teamProjects.ListTeamProjectNames())
            {
                if (string.Compare(project, projectName, true) == 0)
                {
                    actual = true;
                    break;
                }
            }

            Assert.IsTrue(actual);
        }

        [TestMethod]
        [Priority(50)]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateTeamProject_PassInTestProjectDetailsWithNoProcessTemplate_ArgumentExceptionThrown()
        {
            // arrange
            ITeamProjects teamProjects = TeamProjectFactory.CreateTeamProjectMananger(TestConstants.TfsCollectionUri);

            // act
            teamProjects.CreateTeamProject(TestProjectNameBase, TestProjectNameBase + " Description", null, false, false);

            // assert
        }

        [TestMethod]
        [Priority(50)]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteTeamProject_PassInTestProjectDetailsWithNonExistingProjectName_ArgumentExceptionThrown()
        {
            // arrange
            ITeamProjects teamProjects = TeamProjectFactory.CreateTeamProjectMananger(TestConstants.TfsCollectionUri);
            string projectName = TestConstants.TfsTeamProjectName + Guid.NewGuid().ToString("N");

            // act
            teamProjects.DeleteTeamProject(projectName);

            // assert
        }

        [TestMethod]
        [Priority(1)]
        public void GetInstanceFromFactory()
        {
            object teamProjects = TeamProjectFactory.CreateTeamProjectMananger(TestConstants.TfsCollectionUri);

            Assert.IsInstanceOfType(teamProjects, typeof(ITeamProjects));
        }

        [TestMethod]
        [Priority(50)]
        public void ListTeamProjectNames_Defaults_ReturnAIEnumerableOfString()
        {
            // arrange
            ITeamProjects teamProjects = TeamProjectFactory.CreateTeamProjectMananger(TestConstants.TfsCollectionUri);

            // act
            object actual = teamProjects.ListTeamProjectNames();

            // assert
            Assert.IsInstanceOfType(actual, typeof(IEnumerable<string>));
        }

        [TestMethod]
        [Priority(50)]
        public void ListTeamProjectNames_Defaults_ReturnAIEnumerableOfStringContainingMoreThan0ProjectNames()
        {
            // arrange
            ITeamProjects teamProjects = TeamProjectFactory.CreateTeamProjectMananger(TestConstants.TfsCollectionUri);

            // act
            IEnumerable<string> projects = teamProjects.ListTeamProjectNames();

            // assert
            bool actual = false;
            foreach (string project in projects)
            {
                actual = true;
                break;
            }

            Assert.IsTrue(actual);
        }

        #endregion
    }
}