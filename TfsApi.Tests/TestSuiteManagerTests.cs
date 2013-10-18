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

    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TfsApi.TestManagement;
    using TfsApi.TestManagement.Contracts;

    #endregion

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TestSuiteManagerTests
    {
        #region Constants

        private const string TestProjectNameBase = "Project Page Tests";

        #endregion

        #region Public Methods and Operators

        [TestMethod]
        [Priority(50)]
        public void ctor_InstanceOfITestSuiteManagerReturn()
        {
            // arrange

            // act
            ITestSuiteManager obj = TestSuiteManagerFactory.CreateTestSuiteMananger(TestConstants.TfsCollectionUri, TestConstants.DefaultCredentials);

            // assert
            Assert.IsInstanceOfType(obj, typeof(ITestSuiteManager));
        }

        #endregion
    }
}