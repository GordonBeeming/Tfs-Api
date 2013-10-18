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
    public class IterationManagerTests
    {
        #region Public Methods and Operators

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1303:ConstFieldNamesMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AddIteration_passingInProjectDetailsAndAnExistingIterationName_ExceptionThrown()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();
            using (IIterationManager manager = IterationManagerFactory.GetManager(projectDetail))
            {
                const string newIterationName = "Iteration 1";
                DateTime? startDate = DateTime.Now;
                DateTime? endDate = DateTime.Now.AddDays(10);

                // act
                manager.AddNewIteration(newIterationName, startDate, endDate);
            }
        }

        [TestMethod]
        public void AddIteration_passingInProjectDetailsWithEnableOnBacklogEqualsFalse_IterationCountGoesUpByOneAndIterationAddedNotVisibleOnBacklog()
        {
            bool expectedOutput = false;
            this.AddIterationAndCheckEnabledOnBacklog(TestConstants.TfsTeam.TeamName, expectedOutput);
        }

        [TestMethod]
        public void AddIteration_passingInProjectDetailsWithEnableOnBacklogEqualsTrue_IterationCountGoesUpByOneAndIterationAddedVisibleOnBacklog()
        {
            bool expectedOutput = true;
            this.AddIterationAndCheckEnabledOnBacklog(TestConstants.TfsTeam.TeamName, expectedOutput);
        }

        [TestMethod]
        public void AddIteration_passingInProjectDetailsWithThreeLevelsOfIterationsAllCompletelyNew_IterationGoesUpByOneOnTopLevelAndTwoLevelsDownAreCreated()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();
            List<ProjectIteration> initialList;
            List<ProjectIteration> finalList;
            bool fullExpectedPathExists = false;
            using (IIterationManager manager = IterationManagerFactory.GetManager(projectDetail))
            {
                initialList = manager.ListIterations();
                string newIterationName = "Top Level " + GetRandomGuid() + "\\Level Two\\Level Three";
                DateTime? startDate = DateTime.Now;
                DateTime? endDate = DateTime.Now.AddDays(10);

                // act
                manager.AddNewIteration(newIterationName, startDate, endDate);

                // assert
                finalList = manager.ListIterations();

                fullExpectedPathExists = manager.CheckIfPathAlreadyExists(newIterationName);
            }

            int expectedRoot = initialList.Count + 1;
            int actualRoot = finalList.Count;

            // check root level iteration count
            Assert.AreEqual(expectedRoot, actualRoot);

            // check newly added top level node has 1 child and that child has 1 child
            Assert.IsTrue(fullExpectedPathExists);
        }

        [TestMethod]
        public void AddIteration_passingInProjectDetailsWithThreeLevelsOfIterations_IterationCountStaysTheSameButTheChildOfChildIterationCountOfTheFirstIterationGoesUpByOne()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();
            List<ProjectIteration> initialList;
            ProjectIteration initialFirstIteration;
            List<ProjectIteration> finalList;
            ProjectIteration finalFirstIteration;
            using (IIterationManager manager = IterationManagerFactory.GetManager(projectDetail))
            {
                initialList = manager.ListIterations();
                if (initialList.Count == 0)
                {
                    Assert.Fail("No iterations found yet to add a duplication of");
                }
                ProjectIteration[] listOfIterations = initialList.Where(o => o.Children.Count > 0).ToArray();
                if (listOfIterations.Length == 0)
                {
                    Assert.Fail("No iterations found in the first interation yet to add a duplication of");
                }
                listOfIterations = listOfIterations.Where(o => o.Children.Count > 0).ToArray();
                if (listOfIterations.Length == 0)
                {
                    Assert.Fail("The first interation has no children yet to add a duplication of");
                }
                ProjectIteration firstIteration = listOfIterations[0];
                initialFirstIteration = firstIteration.Children[0];
                string newIterationName = firstIteration.Name + "\\" + initialFirstIteration.Name + "\\Iteration " + GetRandomGuid();
                DateTime? startDate = DateTime.Now;
                DateTime? endDate = DateTime.Now.AddDays(10);

                // act
                manager.AddNewIteration(newIterationName, startDate, endDate);

                // assert
                finalList = manager.ListIterations();
                listOfIterations = finalList.Where(o => o.Children.Count > 0).ToArray();
                listOfIterations = listOfIterations.Where(o => o.Children.Count > 0).ToArray();
                finalFirstIteration = listOfIterations[0].Children[0];
            }

            int expectedRoot = initialList.Count;
            int actualRoot = finalList.Count;

            // check root level Area count
            Assert.AreEqual(expectedRoot, actualRoot);

            int expectedChild = initialFirstIteration.Children.Count + 1;
            int actualChild = finalFirstIteration.Children.Count;

            // check child level Area count
            Assert.AreEqual(expectedChild, actualChild);
        }

        [TestMethod]
        public void AddIteration_passingInProjectDetailsWithTwoLevelsOfIterations_IterationCountStaysTheSameButTheChildIterationCountOfTheFirstIterationGoesUpByOne()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();
            List<ProjectIteration> initialList;
            ProjectIteration initialFirstIteration;
            List<ProjectIteration> finalList;
            ProjectIteration finalFirstIteration;
            using (IIterationManager manager = IterationManagerFactory.GetManager(projectDetail))
            {
                initialList = manager.ListIterations();
                initialFirstIteration = initialList[0];
                string newIterationName = initialFirstIteration.Name + "\\Iteration " + GetRandomGuid();
                DateTime? startDate = DateTime.Now;
                DateTime? endDate = DateTime.Now.AddDays(10);

                // act
                manager.AddNewIteration(newIterationName, startDate, endDate);

                // assert
                finalList = manager.ListIterations();
                finalFirstIteration = finalList[0];
            }

            int expectedRoot = initialList.Count;
            int actualRoot = finalList.Count;

            // check root level iteration count
            Assert.AreEqual(expectedRoot, actualRoot);

            int expectedChild = initialFirstIteration.Children.Count + 1;
            int actualChild = finalFirstIteration.Children.Count;

            // check child level iteration count
            Assert.AreEqual(expectedChild, actualChild);
        }

        [TestMethod]
        public void AddIteration_passingInProjectDetails_IterationCountGoesUpByOne()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();
            List<ProjectIteration> initialList;
            List<ProjectIteration> finalList;
            IIterationManager manager = IterationManagerFactory.GetManager(projectDetail);

            initialList = manager.ListIterations();
            string newIterationName = "Iteration " + GetRandomGuid();
            DateTime? startDate = DateTime.Now;
            DateTime? endDate = DateTime.Now.AddDays(10);

            // act
            manager.AddNewIteration(newIterationName, startDate, endDate);

            // assert
            finalList = manager.ListIterations();

            int expected = initialList.Count + 1;
            int actual = finalList.Count;
            Assert.AreEqual(expected, actual);

            manager.Dispose();
        }

        [TestMethod]
        public void Ctor_disposeOfObject_NoErrorThrown()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();

            // act
            using (IIterationManager manager = IterationManagerFactory.GetManager(projectDetail))
            {
                manager.Dispose();
            }

            // assert
            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void Ctor_passingInProjectDetails_InstanceOfIterationManager()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();

            // act
            IIterationManager manager = IterationManagerFactory.GetManager(projectDetail);

            // assert
            Assert.IsInstanceOfType(manager, typeof(IIterationManager));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DeleteIterationUsingIterationPath_passInAInValidIterationPath_ExceptionExpected()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();
            IIterationManager manager = IterationManagerFactory.GetManager(projectDetail);

            // act
            manager.DeleteIterationUsingIterationPath(GetRandomGuid());

            // assert
        }

        [TestMethod]
        public void DeleteIterationUsingIterationPath_passInAValidIterationPath_ListIterationsReturnLess1ThanBeforeDelele()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();
            IIterationManager manager = IterationManagerFactory.GetManager(projectDetail);
            string iterationPath;
            int expected = this.AddIteration(manager, out iterationPath);

            // act
            manager.DeleteIterationUsingIterationPath(iterationPath);

            // assert
            int actual = manager.ListIterations().Count;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteIteration_passInNull_ArgumentNullException()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();
            IIterationManager manager = IterationManagerFactory.GetManager(projectDetail);

            // act
            manager.DeleteIteration(null);

            // assert
        }

        [TestMethod]
        public void IsIterationPathVisibleForIterationPlanning_passingInAPathThatIsAssignedToATeamAfterItHasBeenCreated_ShouldReturnTrue()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();
            ITeamManager teamManager = TeamManagerFactory.GetManager(projectDetail);
            var tfsTeamName = GetRandomGuid() + "Team";
            ITfsTeam tfsTeam = teamManager.AddTeam(tfsTeamName);
            IIterationManager manager = IterationManagerFactory.GetManager(projectDetail);
            string newIterationName = GetRandomGuid();
            manager.AddNewIteration(newIterationName);
            bool actual = false;

            // act
            actual = manager.IsIterationPathEnabled(tfsTeam, newIterationName);

            // assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsIterationPathVisibleForIterationPlanning_passingInAnyPathAfterCreatingANewTeam_ShouldReturnFalse()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();
            ITeamManager teamManager = TeamManagerFactory.GetManager(projectDetail);
            var tfsTeamName = GetRandomGuid() + "Team";
            ITfsTeam tfsTeam = teamManager.AddTeam(tfsTeamName);
            IIterationManager manager = IterationManagerFactory.GetManager(projectDetail);
            bool actual = false;

            // act
            actual = manager.IsIterationPathEnabled(tfsTeam, GetRandomGuid());

            // assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void ListIterations_passingInProjectDetails_InstanceOfListOfIterations()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();
            IIterationManager manager = IterationManagerFactory.GetManager(projectDetail);

            // act
            List<ProjectIteration> list = manager.ListIterations();

            // assert
            Assert.IsInstanceOfType(list, typeof(List<ProjectIteration>));
        }

        [TestMethod]
        public void ListIterations_passingInProjectDetails_IterationListReturnMoreThanZeroResults()
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();
            IIterationManager manager = IterationManagerFactory.GetManager(projectDetail);

            // act
            List<ProjectIteration> list = manager.ListIterations();

            // assert
            Assert.IsTrue(list.Count > 0);
        }


        [TestMethod]
        public void EnableDisableIterationPath_passingInNewPath_ShouldEnablePathAndThenDisablePath()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            IIterationManager manager = IterationManagerFactory.GetManager(projectDetail);
            string newIterationName = GetRandomGuid();
            manager.AddNewIteration(newIterationName);

            manager.EnableIterationPath(TestConstants.TfsTeam, newIterationName, true);
            Assert.IsTrue(manager.IsIterationPathEnabled(TestConstants.TfsTeam, newIterationName), "Iteration path did not enable for team.");
            manager.DisableIterationPath(TestConstants.TfsTeam, newIterationName);
            Assert.IsFalse(manager.IsIterationPathEnabled(TestConstants.TfsTeam, newIterationName), "Iteration path did not disable for team.");
        }
        #endregion

        #region Methods

        private static string GetRandomGuid()
        {
            return Guid.NewGuid().ToString("N");
        }

        private int AddIteration(IIterationManager manager, out string iterationPath)
        {
            int notExpected = manager.ListIterations().Count;
            iterationPath = GetRandomGuid();
            manager.AddNewIteration(iterationPath, null, null);
            int actual = manager.ListIterations().Count;

            Assert.AreNotEqual(notExpected, actual, "Adding an Iteration to delete returned unexpected count");
            return notExpected;
        }

        private void AddIterationAndCheckEnabledOnBacklog(string teamName, bool addToBacklogForTeam)
        {
            // arrange
            ProjectDetail projectDetail = this.CreateProjectDetail();
            List<ProjectIteration> initialList;
            List<ProjectIteration> finalList;
            string newIterationName = null;
            IIterationManager manager = IterationManagerFactory.GetManager(projectDetail);
            ITeamManager teamManager = TeamManagerFactory.GetManager(projectDetail);

            initialList = manager.ListIterations();
            newIterationName = "Iteration " + GetRandomGuid();
            DateTime? startDate = DateTime.Now;
            DateTime? endDate = DateTime.Now.AddDays(10);

            // act
            manager.AddNewIteration(newIterationName, startDate, endDate, new List<ITfsTeam> { addToBacklogForTeam ? teamManager.GetTfsTeam(teamName) : null });

            // assert
            finalList = manager.ListIterations();

            int expected = initialList.Count + 1;
            int actual = finalList.Count;
            Assert.AreEqual(expected, actual);

            ProjectIteration addedItem = (from o in finalList
                                          where o.Name == newIterationName
                                          select o).FirstOrDefault();

            Assert.IsNotNull(addedItem);

            Assert.AreEqual(addToBacklogForTeam, teamManager.GetTfsTeam(teamName).IsIterationPathEnabled(newIterationName));
        }

        private ProjectDetail CreateProjectDetail()
        {
            return new ProjectDetail
            {
                CollectionUri = TestConstants.TfsCollectionUri,
                ProjectName = TestConstants.TfsTeamProjectName
            };
        }

        #endregion
    }

    // ReSharper restore InconsistentNaming
}