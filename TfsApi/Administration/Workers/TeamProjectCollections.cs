// // <copyright file="TeamProjectCollections.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the TeamProjectCollections.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Administration
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.Framework.Common;
    using Microsoft.TeamFoundation.Server;

    using TfsApi.Administration.Contracts;
    using TfsApi.Administration.Dto;
    using TfsApi.Administration.Enums;
    using TfsApi.Contracts;
    using TfsApi.Utilities;

    #endregion

    internal sealed class TeamProjectCollections : ITeamProjectCollections
    {
        #region Fields

        private readonly IRegistration _registration;

        public IRegistration Registration
        {
            get { return _registration; }
        }


        private readonly ITeamProjectCollectionService _teamProjectCollectionService;

        public ITeamProjectCollectionService TeamProjectCollectionService
        {
            get { return _teamProjectCollectionService; }
        }


        private readonly TfsConfigurationServer _tfsConfigurationServer;

        public TfsConfigurationServer TfsConfigurationServer
        {
            get { return _tfsConfigurationServer; }
        }


        private readonly ITfsCredentials _tfsCredentials;

        public ITfsCredentials TfsCredentials
        {
            get { return _tfsCredentials; }
        }


        private readonly TfsTeamProjectCollection _tfsTeamProjectCollection;

        public TfsTeamProjectCollection TfsTeamProjectCollection
        {
            get { return _tfsTeamProjectCollection; }
        } 


        public readonly Uri _tfsUri;

        /// <summary>
        ///     Internal variable which checks if Dispose has already been called
        /// </summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        public TeamProjectCollections(Uri tfsUri, ITfsCredentials tfsCredentials)
        {
            this._tfsUri = tfsUri;
            this._tfsCredentials = tfsCredentials;
            this._tfsTeamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsUri, this._tfsCredentials);
            this._tfsConfigurationServer = this._tfsTeamProjectCollection.GetConfigurationServer();
            this._registration = (IRegistration)this._tfsTeamProjectCollection.GetService(typeof(IRegistration));
            this._teamProjectCollectionService = this._tfsConfigurationServer.GetService<ITeamProjectCollectionService>();
        }

        #endregion

        #region Public Properties

        public Uri TfsUri
        {
            get
            {
                return this._tfsUri;
            }
        }

        #endregion

        #region Public Methods and Operators

        public bool CollectionExists(string collectionName)
        {
            return (from o in this.ListCollections()
                    where string.Compare(o.CollectionName, collectionName, true) == 0
                    select o).FirstOrDefault() != null;
        }

        public void CreateProjectCollection(string name, string description, eSharePointActions sharePointAction = eSharePointActions.None, string sharePointServerHash = null, string sharePointSitePath = null, eReportingActions reportingAction = eReportingActions.None, string reportServerHash = null, string reportFolderPath = null)
        {
            if (!this.CollectionExists(name))
            {
                var servicingTokens = new Dictionary<string, string>();
                if (sharePointAction != eSharePointActions.None)
                {
                    servicingTokens.Add("SharePointAction", sharePointAction.ToString());
                    servicingTokens.Add("SharePointServer", sharePointServerHash);
                    servicingTokens.Add("SharePointSitePath", (sharePointSitePath ?? string.Empty).Replace("~/", name + "/"));
                }
                else
                {
                    servicingTokens.Add("SharePointAction", "None");
                }

                if (reportingAction != eReportingActions.None)
                {
                    servicingTokens.Add("ReportingAction", reportingAction.ToString());
                    servicingTokens.Add("ReportServer", reportServerHash);
                    servicingTokens.Add("ReportFolder", (reportFolderPath ?? string.Empty).Replace("~/", name + "/"));
                }
                else
                {
                    servicingTokens.Add("ReportingAction", "None");
                }

                ServicingJobDetail sjd = this._teamProjectCollectionService.QueueCreateCollection(name, description, false, "~/" + name + "/", TeamFoundationServiceHostStatus.Started, servicingTokens);

                this._teamProjectCollectionService.WaitForCollectionServicingToComplete(sjd);
            }
            else
            {
                throw new Exception("The collection " + name + " already exists in Team Foundation Server.");
            }
        }

        /// <summary>
        ///     Deletes the project collection.
        ///     More Info : http://msdn.microsoft.com/en-us/library/vstudio/ee349263.aspx
        /// </summary>
        /// <param name="teamProjectCollection">The team project collection.</param>
        /// <exception cref="System.Exception">There was an error Deleting the team project ' + teamProjectName + '.\n\n + error</exception>
        public void DeleteProjectCollection(string teamProjectCollection)
        {
            string cmdLines = string.Empty;
            if (!Directory.Exists(@"C:\Program Files\Microsoft Team Foundation Server 11.0\Tools"))
            {
                throw new Exception(@"The path 'C:\Program Files\Microsoft Team Foundation Server 11.0\Tools' doesn't exists");
            }

            cmdLines += @"cd C:\Program Files\Microsoft Team Foundation Server 11.0\Tools" + Environment.NewLine;

            cmdLines += "TFSConfig Collection /collectionName:\"" + teamProjectCollection + "\" /delete" + Environment.NewLine;
            cmdLines += "Yes";

            string output;
            string error;
            CmdUtility.Execute(cmdLines, out output, out error);
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception("There was an error Deleting the team project collection '" + teamProjectCollection + "'.\n\n" + error);
            }

            if (!output.ToLower().Contains("the delete of collection '" + teamProjectCollection.ToLower() + "' succeeded."))
            {
                throw new Exception("No error found but we did not receive a valid message confirming the deletion of the team project collection '" + teamProjectCollection + "'.");
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Call the private Dispose(bool) helper and indicate 
            // that we are explicitly disposing
            this.Dispose(true);

            // Tell the garbage collector that the object doesn't require any
            // cleanup when collected since Dispose was called explicitly.
            GC.SuppressFinalize(this);
        }

        public Uri GetProjectCollectionSharepointUri(string collectionName)
        {
            RegistrationEntry[] entries = this._registration.GetRegistrationEntries("TeamProjects");
            ServiceInterface endpoint = entries[0].ServiceInterfaces.FirstOrDefault(si => si.Name == collectionName + ":Portal");
            if (endpoint == null)
            {
                return null;
            }

            return new Uri(endpoint.Url);
        }

        public List<Collection> ListCollections()
        {
            var result = new List<Collection>();

            CatalogNode catalogNode = this._tfsConfigurationServer.CatalogNode;

            foreach (CatalogNode item in catalogNode.QueryChildren(new[] { CatalogResourceTypes.ProjectCollection }, false, CatalogQueryOptions.None))
            {
                var tpcId = new Guid(item.Resource.Properties["InstanceId"]);
                TfsTeamProjectCollection tpc = this._tfsConfigurationServer.GetTeamProjectCollection(tpcId);

                result.Add(new Collection {
                                              TeamProjectCollectionID = tpcId, 
                                              IsDefault = item.IsDefault, 
                                              CollectionName = tpc.CatalogNode.Resource.DisplayName, 
                                              Url = tpc.Uri, 
                                              CollectionDescription = tpc.CatalogNode.Resource.Description
                                          });
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this._tfsConfigurationServer.Dispose();
                this._tfsTeamProjectCollection.Dispose();
            }

            // TODO: Unmanaged cleanup code here
            this.disposed = true;
        }

        #endregion
    }
}