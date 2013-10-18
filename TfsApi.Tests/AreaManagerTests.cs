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
    public class AreaManagerTests
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1303:ConstFieldNamesMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        [TestMethod]
        public void Ctor_passingInProjectDetails_InstanceOfAreaManager()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;

            // act
            IAreaManager manager = AreaManagerFactory.GetManager(projectDetail);

            // assert
            Assert.IsInstanceOfType(manager, typeof(IAreaManager));
        }

        [TestMethod]
        public void ListAreas_passingInProjectDetails_InstanceOfListOfAreas()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            IAreaManager manager = AreaManagerFactory.GetManager(projectDetail);

            // act
            List<ProjectArea> list = manager.ListAreas();

            // assert
            Assert.IsInstanceOfType(list, typeof(List<ProjectArea>));
        }

        [TestMethod]
        public void AddArea_passingInProjectDetails_AreaCountGoesUpByOne()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            List<ProjectArea> initialList;
            List<ProjectArea> finalList;
            IAreaManager manager = AreaManagerFactory.GetManager(projectDetail);

            initialList = manager.ListAreas();
            string newAreaName = "Area " + GetRandomGuid();

            // act
            manager.AddNewArea(newAreaName);

            // assert
            finalList = manager.ListAreas();

            int expected = initialList.Count + 1;
            int actual = finalList.Count;
            Assert.AreEqual(expected, actual);

            manager.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AddArea_passingInProjectDetailsAndAnExistingAreaName_ExceptionThrown()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            using (IAreaManager manager = AreaManagerFactory.GetManager(projectDetail))
            {
                string newAreaName = GetRandomGuid() + " Area";
                manager.AddNewArea(newAreaName);

                // act
                manager.AddNewArea(newAreaName);
            }
        }

        [TestMethod]
        public void AddArea_passingInProjectDetailsWithThreeLevelsOfAreasAllCompletelyNew_AreaGoesUpByOneOnTopLevelAndTwoLevelsDownAreCreated()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            List<ProjectArea> initialList;
            List<ProjectArea> finalList;
            bool fullExpectedPathExists = false;
            using (IAreaManager manager = AreaManagerFactory.GetManager(projectDetail))
            {
                initialList = manager.ListAreas();
                string newAreaName = "Top Level " + GetRandomGuid() + "\\Level Two\\Level Three";

                // act
                manager.AddNewArea(newAreaName);

                // assert
                finalList = manager.ListAreas();

                fullExpectedPathExists = manager.CheckIfPathAlreadyExists(newAreaName);
            }

            int expectedRoot = initialList.Count + 1;
            int actualRoot = finalList.Count;

            // check root level Area count
            Assert.AreEqual(expectedRoot, actualRoot);

            // check newly added top level node has 1 child and that child has 1 child
            Assert.IsTrue(fullExpectedPathExists);
        }

        [TestMethod]
        public void AddArea_passingInProjectDetailsWithThreeLevelsOfAreas_AreaCountStaysTheSameButTheChildOfChildAreaCountOfTheFirstAreaGoesUpByOne()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            List<ProjectArea> initialList;
            ProjectArea initialFirstArea;
            List<ProjectArea> finalList;
            ProjectArea finalFirstArea;
            using (IAreaManager manager = AreaManagerFactory.GetManager(projectDetail))
            {
                initialList = manager.ListAreas();
                if (initialList.Count == 0)
                {
                    Assert.Fail("No Areas found yet to add a duplication of");
                }
                ProjectArea[] listOfAreas = initialList.Where(o=>o.Children.Count > 0).ToArray();
                if (listOfAreas.Length == 0)
                {
                    Assert.Fail("No Areas found in the first interation yet to add a duplication of");
                }
                listOfAreas = listOfAreas.Where(o=>o.Children.Count > 0).ToArray();
                if (listOfAreas.Length == 0)
                {
                    Assert.Fail("The first interation has no children yet to add a duplication of");
                }
                ProjectArea firstArea = listOfAreas[0];
                initialFirstArea = firstArea.Children[0];
                string newAreaName = firstArea.Name + "\\" + initialFirstArea.Name + "\\Area " + GetRandomGuid();

                // act
                manager.AddNewArea(newAreaName);

                // assert
                finalList = manager.ListAreas();
                listOfAreas = finalList.Where(o => o.Children.Count > 0).ToArray();
                listOfAreas = listOfAreas.Where(o => o.Children.Count > 0).ToArray();
                finalFirstArea = listOfAreas[0].Children[0];
            }

            int expectedRoot = initialList.Count;
            int actualRoot = finalList.Count;

            // check root level Area count
            Assert.AreEqual(expectedRoot, actualRoot);

            int expectedChild = initialFirstArea.Children.Count + 1;
            int actualChild = finalFirstArea.Children.Count;

            // check child level Area count
            Assert.AreEqual(expectedChild, actualChild);
        }

        [TestMethod]
        public void AddArea_passingInProjectDetailsWithTwoLevelsOfAreas_AreaCountStaysTheSameButTheChildAreaCountOfTheFirstAreaGoesUpByOne()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            List<ProjectArea> initialList;
            ProjectArea initialFirstArea;
            List<ProjectArea> finalList;
            ProjectArea finalFirstArea;
            using (IAreaManager manager = AreaManagerFactory.GetManager(projectDetail))
            {
                initialList = manager.ListAreas();
                initialFirstArea = initialList[0];
                string newAreaName = initialFirstArea.Name + "\\Area " + GetRandomGuid();

                // act
                manager.AddNewArea(newAreaName);

                // assert
                finalList = manager.ListAreas();
                finalFirstArea = finalList[0];
            }

            int expectedRoot = initialList.Count;
            int actualRoot = finalList.Count;

            // check root level Area count
            Assert.AreEqual(expectedRoot, actualRoot);

            int expectedChild = initialFirstArea.Children.Count + 1;
            int actualChild = finalFirstArea.Children.Count;

            // check child level Area count
            Assert.AreEqual(expectedChild, actualChild);
        }

        [TestMethod]
        public void Ctor_disposeOfObject_NoErrorThrown()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;

            // act
            using (IAreaManager manager = AreaManagerFactory.GetManager(projectDetail))
            {
                manager.Dispose();
            }

            // assert
            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DeleteAreaUsingAreaPath_passInAInValidAreaPath_ExceptionExpected()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            IAreaManager manager = AreaManagerFactory.GetManager(projectDetail);

            // act
            manager.DeleteAreaUsingAreaPath(GetRandomGuid());

            // assert
        }

        [TestMethod]
        public void DeleteAreaUsingAreaPath_passInAValidAreaPath_ListAreasReturnLess1ThanBeforeDelele()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            IAreaManager manager = AreaManagerFactory.GetManager(projectDetail);
            string AreaPath;
            int expected = this.AddArea(manager, out AreaPath);

            // act
            manager.DeleteAreaUsingAreaPath(AreaPath);

            // assert
            int actual = manager.ListAreas().Count;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteArea_passInNull_ArgumentNullException()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            IAreaManager manager = AreaManagerFactory.GetManager(projectDetail);

            // act
            manager.DeleteArea(null);

            // assert
        }

        [TestMethod]
        public void IsAreaPathVisibleForAreaPlanning_passingInCorrectPath_ReturnTrue()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            IAreaManager manager = AreaManagerFactory.GetManager(projectDetail);
            string newAreaName = GetRandomGuid();
            manager.AddNewArea(newAreaName);
            bool actual = false;

            // act
            actual = manager.IsAreaPathEnabled(TestConstants.TfsTeam, newAreaName);

            // assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void EnableDisableAreaPath_passingInNewPath_ShouldEnablePathAndThenDisablePath()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            IAreaManager manager = AreaManagerFactory.GetManager(projectDetail);
            string newAreaName = GetRandomGuid();
            manager.AddNewArea(newAreaName);

            manager.EnableAreaPath(TestConstants.TfsTeam, newAreaName, true);
            Assert.IsTrue(manager.IsAreaPathEnabled(TestConstants.TfsTeam, newAreaName), "Area path did not enable for team.");
            manager.DisableAreaPath(TestConstants.TfsTeam, newAreaName);
            Assert.IsFalse(manager.IsAreaPathEnabled(TestConstants.TfsTeam, newAreaName), "Area path did not disable for team.");
        }

        [TestMethod]
        public void IsAreaPathVisibleForAreaPlanning_passingInWrongPath_ReturnFalse()
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            IAreaManager manager = AreaManagerFactory.GetManager(projectDetail);
            bool actual = false;

            // act
            actual = manager.IsAreaPathEnabled(TestConstants.TfsTeam, GetRandomGuid());

            // assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void AddArea_passingInProjectDetailsWithEnableOnBacklogEqualsFalse_AreaCountGoesUpByOneAndAreaAddedNotVisibleOnBacklog()
        {
            bool expectedOutput = false;
            this.AddAreaAndCheckEnabledOnBacklog(TestConstants.TfsTeam.TeamName, expectedOutput);
        }

        [TestMethod]
        public void AddArea_passingInProjectDetailsWithEnableOnBacklogEqualsTrue_AreaCountGoesUpByOneAndAreaAddedVisibleOnBacklog()
        {
            bool expectedOutput = true;
            this.AddAreaAndCheckEnabledOnBacklog(TestConstants.TfsTeam.TeamName, expectedOutput);
        }

        private void AddAreaAndCheckEnabledOnBacklog(string teamName, bool enableForTeam)
        {
            // arrange
            ProjectDetail projectDetail = TestConstants.TfsTeamProjectDetail;
            List<ProjectArea> initialList;
            List<ProjectArea> finalList;
            string newAreaName = null;
            IAreaManager manager = AreaManagerFactory.GetManager(projectDetail);
            ITeamManager teamManager = TeamManagerFactory.GetManager(projectDetail);

            initialList = manager.ListAreas();
            newAreaName = "Area " + GetRandomGuid();

            // act
            manager.AddNewArea(newAreaName, new List<ITfsTeam> { enableForTeam ? teamManager.GetTfsTeam(teamName) : null });

            // assert
            finalList = manager.ListAreas();

            int expected = initialList.Count + 1;
            int actual = finalList.Count;
            Assert.AreEqual(expected, actual);

            ProjectArea addedItem = (from o in finalList
                                          where o.Name == newAreaName
                                          select o).FirstOrDefault();

            Assert.IsNotNull(addedItem);

            Assert.AreEqual(enableForTeam, teamManager.GetTfsTeam(teamName).IsAreaPathEnabled(newAreaName));
        }

        private static string GetRandomGuid()
        {
            return Guid.NewGuid().ToString("N");
        }

        private int AddArea(IAreaManager manager, out string areaPath)
        {
            int notExpected = manager.ListAreas().Count;
            areaPath = GetRandomGuid();
            manager.AddNewArea(areaPath);
            int actual = manager.ListAreas().Count;

            Assert.AreNotEqual(notExpected, actual, "Adding an Area to delete returned unexpected count");
            return notExpected;
        }

    }

    // ReSharper restore InconsistentNaming
}