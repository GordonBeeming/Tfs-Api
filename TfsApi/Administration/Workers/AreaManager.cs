using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TfsApi.Administration.Contracts;
using TfsApi.Administration.Dto;
using TfsApi.Administration.Enums;
using TfsApi.Administration.Helpers;
using TfsApi.Contracts;

namespace TfsApi.Administration.Workers
{
    internal class AreaManager : IAreaManager
    {
        /// <summary>
        ///     Internal variable which checks if Dispose has already been called
        /// </summary>
        private bool disposed;

        private readonly ICommonStructureService4 commonStructureService;

        private readonly ProjectDetail projectDetail;

        private readonly ProjectInfo projectInfo;

        private readonly TfsTeamProjectCollection tfsTeamProjectCollection;

        private readonly TeamSettingsConfigurationService teamConfig;

        private readonly TeamConfiguration teamConfiguration;

        public AreaManager(ProjectDetail projectDetail, ITfsCredentials tfsCredentials)
        {
            this.projectDetail = projectDetail;

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

        public List<ProjectArea> FlattenAreas(List<ProjectArea> list)
        {
            var result = new List<ProjectArea>();

            foreach (ProjectArea item in list)
            {
                result.Add(item);
                if (item.Children != null)
                {
                    result.AddRange(this.FlattenAreas(item.Children));
                }
            }

            return result;
        }

        public List<ProjectArea> ListAreas()
        {
            var result = new List<ProjectArea>();

            XmlElement allNodesXml = this.GetAreaXml();
            result = this.LoadAreas(allNodesXml);

            return result[0].Children;
        }

        private XmlElement GetAreaXml()
        {
            ProjectInfo projectInfo = this.commonStructureService.GetProjectFromName(this.projectDetail.ProjectName);
            NodeInfo[] nodes = this.commonStructureService.ListStructures(projectInfo.Uri);
            string uri = nodes.First(o => o.Name == "Area").Uri;
            XmlElement iterationsTree = this.commonStructureService.GetNodesXml(new[] { uri }, true);
            return iterationsTree;
        }

        private List<ProjectArea> LoadAreas(XmlNode parentNode, ProjectArea parentItem = null)
        {
            var result = new List<ProjectArea>();

            foreach (XmlNode item in parentNode.ChildNodes)
            {
                if (item.Name == "Children")
                {
                    result = this.LoadAreas(item, parentItem);
                }
                else
                {
                    string nodeId = this.GetNodeID(item.OuterXml);
                    NodeInfo nodeInfo = this.commonStructureService.GetNode(nodeId);

                    var pi = new ProjectArea
                    {
                        Name = nodeInfo.Name,
                        ParentUri = nodeInfo.ParentUri,
                        Path = nodeInfo.Path,
                        ProjectUri = nodeInfo.ProjectUri,
                        StructureType = nodeInfo.StructureType,
                        Uri = nodeInfo.Uri,
                        FullPath = this.GetFullPath(parentItem, nodeInfo.Name),
                        ParentProjectArea = parentItem
                    };
                    pi.Children = this.LoadAreas(item, pi);
                    result.Add(pi);
                }
            }

            return result;
        }

        private string GetFullPath(ProjectArea parentItem, string name)
        {
            string result = name;

            while (parentItem != null)
            {
                result = parentItem.Name + "\\" + result;
                parentItem = parentItem.ParentProjectArea;
            }

            if (result.ToLower().StartsWith("area\\"))
            {
                result = result.Remove(0, result.IndexOf('\\') + 1);
            }

            return this.FormatAreaName(result);
        }

        private string GetNodeID(string xml)
        {
            string first = "NodeID=\"";
            int start = xml.IndexOf(first) + first.Length;
            int end = xml.IndexOf("\"", start);
            return xml.Substring(start, end - start);
        }

        public void AddNewArea(string areaPath, List<ITfsTeam> enableforTfsTeams = null, bool includeChildren = true, bool refreshCache =false)
        {
            NodeInfo retVal = this.AddArea(areaPath);

            if (enableforTfsTeams != null)
            {
                foreach (var team in enableforTfsTeams)
                {
                    if (team != null)
                    {
                        this.AddAreaToTeamAreas(team, areaPath, includeChildren);
                    }
                }
            }
            RefreshCache(refreshCache);
        }

        private void RefreshCache(bool refreshCache)
        {
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

        public void AddNewArea(string areaPath, Dictionary<ITfsTeam, bool> enableforTfsTeams, bool refreshCache = false)
        {
            NodeInfo retVal = this.AddArea(areaPath);

            if (enableforTfsTeams != null)
            {
                foreach (var team in enableforTfsTeams)
                {
                    if (team.Key != null)
                    {
                        this.AddAreaToTeamAreas(team.Key, areaPath, team.Value);
                    }
                }
            }

            RefreshCache(refreshCache);
        }

        private void AddAreaToTeamAreas(ITfsTeam tfsTeam, string areaPath, bool includeChildren)
        {
            tfsTeam.EnableAreaPath(areaPath, includeChildren);
        }

        private NodeInfo AddArea(string areaPath)
        {
            NodeInfo retVal = TfsNodeStructureHelper.AddNode(this.commonStructureService, "\\" + areaPath, this.projectDetail.ProjectName, eStructureType.Area);
            if (retVal == null)
            {
                throw new Exception("We were unable to add the area '" + areaPath + "' to the project '" + this.projectDetail.ProjectName + "'!");
            }

            return retVal;
        }


        public bool CheckIfPathAlreadyExists(string newAreaName)
        {
            // return TfsNodeStructureHelper.CheckIfPathAlreadyExists(this.commonStructureService, iterationPath, this.projectDetail.ProjectName + "\\Iteration");
            string formattedPath = this.FormatAreaName(newAreaName);
            return this.FlattenAreas(this.ListAreas()).Any(item => string.Compare(item.FullPath, formattedPath, true) == 0);
        }


        public void EnableAreaPath(ITfsTeam tfsTeam, string areaName, bool includeChildren)
        {
            tfsTeam.EnableAreaPath(areaName, includeChildren);
        }

        public void DisableAreaPath(ITfsTeam tfsTeam, string areaName)
        {
            tfsTeam.DisableAreaPath(areaName);
        }


        public bool IsAreaPathEnabled(ITfsTeam tfsTeam, string areaName)
        {
            return tfsTeam.IsAreaPathEnabled(areaName);
        }


        public void DeleteArea(ProjectArea projectArea, ProjectArea reAssignArea = null)
        {
            if (projectArea != null)
            {
                ProjectArea newProjectArea = reAssignArea ?? projectArea.ParentProjectArea;
                this.commonStructureService.DeleteBranches(new[] { projectArea.Uri }, newProjectArea.Uri);
            }
            else
            {
                throw new ArgumentNullException("projectArea", "A valid Project Area is required to perform a delete");
            }
        }

        public void DeleteAreaUsingAreaPath(string areaPath, string reAssignAreaPath = null)
        {
            if (this.CheckIfPathAlreadyExists(areaPath))
            {
                string formattedAreaPath = this.FormatAreaName(areaPath);
                ProjectArea projectArea = this.FindProjectArea(formattedAreaPath);
                ProjectArea newProjectArea = projectArea.ParentProjectArea;
                if (!string.IsNullOrEmpty(reAssignAreaPath))
                {
                    newProjectArea = this.FindProjectArea(reAssignAreaPath);
                    if (newProjectArea == null)
                    {
                        throw new ArgumentOutOfRangeException("No path exists for " + reAssignAreaPath);
                    }
                }
                this.DeleteArea(projectArea, newProjectArea);
            }
            else
            {
                throw new Exception("The iteration path " + areaPath + " doesn't exists in the project " + this.projectDetail.ProjectName);
            }
        }

        public ProjectArea FindProjectArea(string fullAreaPath)
        {
            ProjectArea projectArea = null;

            foreach (ProjectArea pa in this.FlattenAreas(this.ListAreas()))
            {
                if (string.Compare(pa.FullPath, fullAreaPath, true) == 0)
                {
                    projectArea = pa;
                    break;
                }
            }

            return projectArea;
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

        private string FormatAreaName(string newAreaName)
        {
            return string.Format(@"{0}\{1}", this.projectDetail.ProjectName, newAreaName.TrimStart('\\'));
        }

    }
}
