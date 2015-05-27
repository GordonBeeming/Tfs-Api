namespace TfsApi.Administration.Workers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.ProcessConfiguration.Client;
    using Microsoft.TeamFoundation.Server;

    using TfsApi.Administration.Contracts;
    using TfsApi.Administration.Dto;
    using TfsApi.Administration.Enums;
    using TfsApi.Administration.Helpers;
    using TfsApi.Contracts;
    using Microsoft.TeamFoundation.WorkItemTracking.Proxy;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    #endregion

    /// <summary>
    ///     The Iteration manager.
    /// </summary>
    internal class IterationManager : IIterationManager, IDisposable
    {
        #region Fields

        private readonly ICommonStructureService4 commonStructureService;

        /// <summary>
        ///     The project detail.
        /// </summary>
        private readonly ProjectDetail projectDetail;

        private readonly ProjectInfo projectInfo;

        private readonly TfsTeamProjectCollection tfsTeamProjectCollection;

        private readonly TeamSettingsConfigurationService teamConfig;

        private readonly TeamConfiguration teamConfiguration;

        /// <summary>
        ///     Internal variable which checks if Dispose has already been called
        /// </summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IterationManager" /> class.
        /// </summary>
        /// <param name="projectDetail">
        ///     The project detail.
        /// </param>
        public IterationManager(ProjectDetail projectDetail, ITfsCredentials tfsCredentials = null)
        {
            this.projectDetail = projectDetail;

            //this.server = TfsTeamFoundationServerFactory.GetTeamFoundationServer(this.projectDetail.CollectionUri, tfsCredentials);

            this.tfsTeamProjectCollection = TfsApi.Administration.TfsTeamProjectCollectionFactory.GetTeamProjectCollection(this.projectDetail.CollectionUri, tfsCredentials);

            this.teamConfig = this.tfsTeamProjectCollection.GetService<TeamSettingsConfigurationService>();

            this.commonStructureService = (ICommonStructureService4)this.tfsTeamProjectCollection.GetService(typeof(ICommonStructureService4));

            this.projectInfo = this.commonStructureService.GetProjectFromName(this.projectDetail.ProjectName);

            foreach (TeamConfiguration item in this.teamConfig.GetTeamConfigurationsForUser(new[] { this.projectInfo.Uri }))
            {
                this.teamConfiguration = item;
                break;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void AddNewIteration(string newIterationName, DateTime? startDate = null, DateTime? finishDate = null, List<ITfsTeam> enableforTfsTeams = null, bool refreshCache = false)
        {
            NodeInfo retVal = this.AddIteration(newIterationName);

            this.SetIterationDates(startDate, finishDate, retVal);

            if (enableforTfsTeams != null)
            {
                foreach (var team in enableforTfsTeams)
                {
                    if (team != null)
                    {
                        this.AddIterationToPlanningIterations(team, newIterationName);
                    }
                }
            }
            if (refreshCache)
            {
                WorkItemServer witProxy = (WorkItemServer)this.tfsTeamProjectCollection.GetService(typeof(WorkItemServer));
                //Get the team project
                ProjectInfo projInfo = commonStructureService.GetProjectFromName(projectDetail.ProjectName);
                //Sync External Store
                witProxy.SyncExternalStructures(WorkItemServer.NewRequestId(), projInfo.Uri);
                //Refresh local cache
                ((WorkItemStore)this.tfsTeamProjectCollection.GetService(typeof(WorkItemStore))).RefreshCache();
            }
        }

        public bool CheckIfPathAlreadyExists(string iterationPath)
        {
            // return TfsNodeStructureHelper.CheckIfPathAlreadyExists(this.commonStructureService, iterationPath, this.projectDetail.ProjectName + "\\Iteration");
            string formattedPath = this.FormatIterationName(iterationPath);
            return this.FlattenIterations(this.ListIterations()).Any(item => string.Compare(item.FullPath, formattedPath, true) == 0);
        }

        public void DeleteIteration(ProjectIteration projectIteration)
        {
            if (projectIteration != null)
            {
                this.commonStructureService.DeleteBranches(new[] { projectIteration.Uri }, projectIteration.ParentUri);
            }
            else
            {
                throw new ArgumentNullException("projectIteration", "A valid Project Iteration is required to perform a delete");
            }
        }

        public void DeleteIterationUsingIterationPath(string iterationPath)
        {
            if (this.CheckIfPathAlreadyExists(iterationPath))
            {
                string formattedIterationPath = this.FormatIterationName(iterationPath);
                ProjectIteration projectIteration = this.FindProjectIteration(formattedIterationPath);
                this.DeleteIteration(projectIteration);
            }
            else
            {
                throw new Exception("The iteration path " + iterationPath + " doesn't exists in the project " + this.projectDetail.ProjectName);
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

        public List<ProjectIteration> FlattenIterations(List<ProjectIteration> list)
        {
            var result = new List<ProjectIteration>();

            foreach (ProjectIteration item in list)
            {
                result.Add(item);
                if (item.Children != null)
                {
                    result.AddRange(this.FlattenIterations(item.Children));
                }
            }

            return result;
        }

        public bool IsIterationPathEnabled(ITfsTeam tfsTeam, string iterationPath)
        {
            return tfsTeam.IsIterationPathEnabled(iterationPath);
        }

        /// <summary>
        ///     The list Iterations.
        /// </summary>
        /// <returns>
        ///     The <see cref="List{T}" />.
        /// </returns>
        public List<ProjectIteration> ListIterations()
        {
            var result = new List<ProjectIteration>();

            XmlElement allNodesXml = this.GetIterationXml();
            result = this.LoadIterations(allNodesXml);

            // return the list of iterations after the default Iteration Node
            return result[0].Children;
        }

        public ProjectIteration FindProjectIteration(string fullIterationPath)
        {
            ProjectIteration projectIteration = null;

            foreach (ProjectIteration pi in this.FlattenIterations(this.ListIterations()))
            {
                if (string.Compare(pi.FullPath, fullIterationPath, true) == 0)
                {
                    projectIteration = pi;
                    break;
                }
            }

            return projectIteration;
        }


        public void EnableIterationPath(ITfsTeam tfsTeam, string iterationName, bool includeChildren)
        {
            tfsTeam.EnableIterationPath(iterationName);
        }

        public void DisableIterationPath(ITfsTeam tfsTeam, string iterationName)
        {
            tfsTeam.DisableIterationPath(iterationName);
        }


        #endregion

        #region Methods

        private NodeInfo AddIteration(string newIterationName)
        {
            NodeInfo retVal = TfsNodeStructureHelper.AddNode(this.commonStructureService, "\\" + newIterationName, this.projectDetail.ProjectName, eStructureType.Iteration);
            if (retVal == null)
            {
                throw new Exception("We were unable to add the iteration '" + newIterationName + "' to the project '" + this.projectDetail.ProjectName + "'!");
            }

            return retVal;
        }

        private void AddIterationToPlanningIterations(ITfsTeam tfsTeam, string iterationName)
        {
            tfsTeam.EnableIterationPath(iterationName);
        }

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
                this.tfsTeamProjectCollection.Dispose();
            }

            this.disposed = true;
        }

        private string FormatIterationName(string newIterationName)
        {
            return string.Format(@"{0}\{1}", this.projectDetail.ProjectName, newIterationName.TrimStart('\\'));
        }

        private string GetFullPath(ProjectIteration parentItem, string name)
        {
            string result = name;

            while (parentItem != null)
            {
                result = parentItem.Name + "\\" + result;
                parentItem = parentItem.ParentProjectIteration;
            }

            if (result.ToLower().StartsWith("iteration\\"))
            {
                result = result.Remove(0, result.IndexOf('\\') + 1);
            }

            return this.FormatIterationName(result);
        }

        private XmlElement GetIterationXml()
        {
            ProjectInfo projectInfo = this.commonStructureService.GetProjectFromName(this.projectDetail.ProjectName);
            NodeInfo[] nodes = this.commonStructureService.ListStructures(projectInfo.Uri);

            XmlElement iterationsTree = this.commonStructureService.GetNodesXml(new[] { nodes.First(o => o.Name == "Iteration").Uri }, true);
            return iterationsTree;
        }

        private string GetNodeID(string xml)
        {
            string first = "NodeID=\"";
            int start = xml.IndexOf(first) + first.Length;
            int end = xml.IndexOf("\"", start);
            return xml.Substring(start, end - start);
        }

        private List<ProjectIteration> LoadIterations(XmlNode parentNode, ProjectIteration parentItem = null)
        {
            var result = new List<ProjectIteration>();

            foreach (XmlNode item in parentNode.ChildNodes)
            {
                if (item.Name == "Children")
                {
                    result = this.LoadIterations(item, parentItem);
                }
                else
                {
                    string nodeId = this.GetNodeID(item.OuterXml);
                    NodeInfo nodeInfo = this.commonStructureService.GetNode(nodeId);

                    var pi = new ProjectIteration
                    {
                        Name = nodeInfo.Name,
                        StartDate = nodeInfo.StartDate,
                        FinishDate = nodeInfo.FinishDate,
                        ParentUri = nodeInfo.ParentUri,
                        Path = nodeInfo.Path,
                        ProjectUri = nodeInfo.ProjectUri,
                        StructureType = nodeInfo.StructureType,
                        Uri = nodeInfo.Uri,
                        FullPath = this.GetFullPath(parentItem, nodeInfo.Name),
                        ParentProjectIteration = parentItem
                    };
                    pi.Children = this.LoadIterations(item, pi);
                    result.Add(pi);
                }
            }

            return result;
        }

        private void SetIterationDates(DateTime? startDate, DateTime? finishDate, NodeInfo retVal)
        {
            this.commonStructureService.SetIterationDates(retVal.Uri, startDate, finishDate);
        }

        #endregion
    }
}