// // <copyright file="ITeamProjectCollections.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the ITeamProjectCollections.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Administration.Contracts
{
    #region

    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.Server;
    using System;
    using System.Collections.Generic;
    using TfsApi.Administration.Dto;
    using TfsApi.Administration.Enums;
    using TfsApi.Contracts;

    #endregion

    public interface ITeamProjectCollections : IDisposable
    {
        #region Public Properties

        /// <summary>
        ///     Gets the TFS URI being used in this instance.
        /// </summary>
        /// <value>
        ///     The TFS URI.
        /// </value>
        Uri TfsUri { get; }

        IRegistration Registration { get; }

        ITeamProjectCollectionService TeamProjectCollectionService { get; }

        TfsConfigurationServer TfsConfigurationServer { get; }

        ITfsCredentials TfsCredentials { get; }

        TfsTeamProjectCollection TfsTeamProjectCollection { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Checks if the collection name exists.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns></returns>
        bool CollectionExists(string collectionName);

        /// <summary>
        ///     Creates the project collection.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="sharePointAction">The share point action.</param>
        /// <param name="sharePointServerHash">The share point server hash.</param>
        /// <param name="sharePointSitePath">The share point site path.</param>
        /// <param name="reportingAction">The reporting action.</param>
        /// <param name="reportServerHash">The report server hash.</param>
        /// <param name="reportFolderPath">The report folder path.</param>
        void CreateProjectCollection(string name, string description, eSharePointActions sharePointAction = eSharePointActions.None, string sharePointServerHash = null, string sharePointSitePath = null, eReportingActions reportingAction = eReportingActions.None, string reportServerHash = null, string reportFolderPath = null);

        void DeleteProjectCollection(string teamProjectCollection);

        /// <summary>
        ///     Gets all collections.
        /// </summary>
        /// <returns></returns>
        List<Collection> ListCollections();

        #endregion
    }
}