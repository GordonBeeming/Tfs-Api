namespace TfsApi.Tests
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TfsApi.Administration;
    using TfsApi.Administration.Contracts;
    using TfsApi.Administration.Dto;
    using TfsApi.Administration.Tests;

    #endregion

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TeamProjectCollectionTests
    {
        #region Public Methods and Operators

        [TestMethod]
        [Priority(0)]
        public void CheckInstanceFromFactory()
        {
            // act
            object teamProjectCollections = TeamProjectCollectionFactory.CreateTeamProjectCollectionMananger(TestConstants.TfsUri, TestConstants.DefaultCredentials);

            // assert
            Assert.IsInstanceOfType(teamProjectCollections, typeof(ITeamProjectCollections));
        }

        [TestMethod]
        [Priority(50)]
        public void CollectionExists_CollectionThatDoesnotExist_ReturnFalse()
        {
            // arrange
            ITeamProjectCollections teamProjectCollections = TeamProjectCollectionFactory.CreateTeamProjectCollectionMananger(TestConstants.TfsUri);
            string collectionName = Guid.NewGuid().ToString("N");

            // act
            bool actual = teamProjectCollections.CollectionExists(collectionName);

            // assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        [Priority(50)]
        public void CollectionExists_CollectionThatExist_ReturnTrue()
        {
            // arrange
            ITeamProjectCollections teamProjectCollections = TeamProjectCollectionFactory.CreateTeamProjectCollectionMananger(TestConstants.TfsUri);
            string collectionName = TestConstants.TfsCollectionName;

            // act
            bool actual = teamProjectCollections.CollectionExists(collectionName);

            // assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        [Priority(50)]
        public void ListCollection_Default_ReturnListOfCollection()
        {
            // arrange
            ITeamProjectCollections teamProjectCollections = TeamProjectCollectionFactory.CreateTeamProjectCollectionMananger(TestConstants.TfsUri);
            string collectionName = TestConstants.TfsCollectionName;

            // act
            object actual = teamProjectCollections.ListCollections();

            // assert
            Assert.IsInstanceOfType(actual, typeof(List<Collection>));
        }

        [TestMethod]
        [Priority(50)]
        public void ListCollection_Default_ReturnListOfCollectionGreaterThan0()
        {
            // arrange
            ITeamProjectCollections teamProjectCollections = TeamProjectCollectionFactory.CreateTeamProjectCollectionMananger(TestConstants.TfsUri);
            string collectionName = TestConstants.TfsCollectionName;

            // act
            List<Collection> actual = teamProjectCollections.ListCollections();

            // assert
            Assert.IsTrue(actual.Count > 0);
        }

        #endregion

        // The Delete Test can't be run because it needs to execute on the TFS Server.
        // [TestMethod]
        // [Priority(99)]
        // public void CreateAndDeleteTeamProjectCollection_UseTestingDefaults_CollectionIsCreatedAndThenCollectionIsRemoved()
        // {
        // // arrange
        // ITeamProjectCollections teamProjectCollections = TeamProjectCollectionFactory.CreateTeamProjectCollectionMananger(TestConstants.TfsUri);
        // string collectionName = TestConstants.TfsCollectionName;

        // // act
        // teamProjectCollections.DeleteProjectCollection(collectionName);

        // // assert
        // bool actualAfter = false;
        // foreach (var collection in teamProjectCollections.ListCollections())
        // {
        // if (string.Compare(collection.CollectionName, collectionName, true) == 0)
        // {
        // actualAfter = true;
        // break;
        // }
        // }

        // Assert.IsFalse(actualAfter, "Delete Failed.");
        // }
    }
}