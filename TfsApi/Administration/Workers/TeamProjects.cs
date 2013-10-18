// // <copyright file="TeamProjects.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the TeamProjects.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Administration
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;

    using EnvDTE80;

    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Server;

    using TfsApi.Administration.Contracts;
    using TfsApi.Administration.Dto;
    using TfsApi.Contracts;
    using TfsApi.Utilities;

    using IProcessTemplates = TfsApi.Administration.Contracts.IProcessTemplates;

    #endregion

    internal sealed class TeamProjects : ITeamProjects
    {
        #region Fields

        private readonly ICommonStructureService _commonStructureService;

        private readonly IRegistration _registration;

        private readonly Uri _tfsCollectionUri;

        private readonly TfsTeamProjectCollection _tfsTeamProjectCollection;

        /// <summary>
        ///     Internal variable which checks if Dispose has already been called
        /// </summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TeamProjects" /> class.
        /// </summary>
        /// <param name="tfsCollectionUri">The TFS collection URI.</param>
        /// <param name="tfsCredentials">The TFS credentials.</param>
        public TeamProjects(Uri tfsCollectionUri, ITfsCredentials tfsCredentials)
        {
            this._tfsCollectionUri = tfsCollectionUri;
            this._tfsTeamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsCollectionUri, tfsCredentials);

            this._registration = (IRegistration)this._tfsTeamProjectCollection.GetService(typeof(IRegistration));
            this._commonStructureService = this._tfsTeamProjectCollection.GetService<ICommonStructureService>();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the team project.
        /// </summary>
        /// <param name="teamProjectName">Name of the team project.</param>
        /// <param name="teamProjectDescription">The team project description.</param>
        /// <param name="processTemplate">The process template.</param>
        /// <param name="createSharepointSite">
        ///     if set to <c>true</c> [create sharepoint site].
        /// </param>
        /// <param name="createReportsSite">
        ///     if set to <c>true</c> [create reports site].
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        ///     No team project name was specified;teamProjectName
        ///     or
        ///     The project name ( + teamProjectName + ) already exists.;teamProjectName
        ///     or
        ///     No invalid process template was specified;processTemplate
        ///     or
        ///     An invalid process template name ( + processTemplate + ) was specified.;processTemplate
        /// </exception>
        /// <exception cref="System.Exception">
        ///     Visual Studio 2012 IDE doesn't exist on the current machine
        ///     or
        ///     The project  + teamProjectName +  already exists in Team Foundation Server Collection  + this._tfsCollectionUri + .
        /// </exception>
        public bool CreateTeamProject(string teamProjectName, string teamProjectDescription, string processTemplate, bool createSharepointSite, bool createReportsSite)
        {
            string refParam = string.Empty;
            return this.CreateTeamProject(teamProjectName, teamProjectDescription, processTemplate, createSharepointSite, createReportsSite, ref refParam);
        }

        /// <summary>
        ///     Creates the team project.
        /// </summary>
        /// <param name="teamProjectName">Name of the team project.</param>
        /// <param name="teamProjectDescription">The team project description.</param>
        /// <param name="processTemplate">The process template.</param>
        /// <param name="createSharepointSite">
        ///     if set to <c>true</c> [create sharepoint site].
        /// </param>
        /// <param name="createReportsSite">
        ///     if set to <c>true</c> [create reports site].
        /// </param>
        /// <param name="logOutput">The log output.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        ///     No team project name was specified;teamProjectName
        ///     or
        ///     The project name ( + teamProjectName + ) already exists.;teamProjectName
        ///     or
        ///     No invalid process template was specified;processTemplate
        ///     or
        ///     An invalid process template name ( + processTemplate + ) was specified.;processTemplate
        /// </exception>
        /// <exception cref="System.Exception">
        ///     Visual Studio 2012 IDE doesn't exist on the current machine
        ///     or
        ///     The project  + teamProjectName +  already exists in Team Foundation Server Collection  + this._tfsCollectionUri + .
        /// </exception>
        public bool CreateTeamProject(string teamProjectName, string teamProjectDescription, string processTemplate, bool createSharepointSite, bool createReportsSite, ref string logOutput)
        {
            if (string.IsNullOrEmpty(teamProjectName))
            {
                throw new ArgumentException("No team project name was specified", "teamProjectName");
            }
            else
            {
                bool foundProjectName = false;
                foreach (string projectname in this.ListTeamProjectNames())
                {
                    if (string.Compare(projectname, teamProjectName, true) == 0)
                    {
                        foundProjectName = true;
                        break;
                    }
                }

                if (foundProjectName)
                {
                    throw new ArgumentException("The project name (" + teamProjectName + ") already exists.", "teamProjectName");
                }
            }

            if (string.IsNullOrEmpty(processTemplate))
            {
                throw new ArgumentException("No invalid process template was specified", "processTemplate");
            }
            else
            {
                IProcessTemplates processTemplates = ProcessTemplateFactory.CreateProcessTemplateMananger(this._tfsCollectionUri);
                bool foundProcessTemplateName = false;
                foreach (ProcessTemplate item in processTemplates.ListProcessTemplates())
                {
                    if (string.Compare(item.Name, processTemplate, true) == 0)
                    {
                        foundProcessTemplateName = true;
                        break;
                    }
                }

                if (!foundProcessTemplateName)
                {
                    throw new ArgumentException("An invalid process template name (" + processTemplate + ") was specified.", "processTemplate");
                }
            }

            bool result = false;
            logOutput = string.Empty;

            if (!this.TeamProjectExists(teamProjectName))
            {
                string logPath = Path.GetTempPath();

                var xmlString = new StringBuilder();
                xmlString.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?> ");
                xmlString.AppendLine("<Project xmlns=\"ProjectCreationSettingsFileSchema.xsd\"> ");
                xmlString.AppendLine(this.EncodeForXml(this._tfsCollectionUri.ToString(), "TFSName"));
                xmlString.AppendLine(this.EncodeForXml(logPath, "LogFolder"));
                xmlString.AppendLine(this.EncodeForXml(teamProjectName, "TFSName"));
                xmlString.AppendLine(this.EncodeForXml(teamProjectName, "ProjectName"));
                xmlString.AppendLine("<ProjectSiteEnabled>" + createSharepointSite.ToString().ToLower() + "</ProjectSiteEnabled> ");
                xmlString.AppendLine(this.EncodeForXml(teamProjectName, "ProjectSiteTitle"));
                xmlString.AppendLine(this.EncodeForXml(teamProjectDescription, "ProjectSiteDescription"));
                xmlString.AppendLine("<SccCreateType>New</SccCreateType> ");
                xmlString.AppendLine("<SccBranchFromPath></SccBranchFromPath>");
                xmlString.AppendLine(this.EncodeForXml(processTemplate, "ProcessTemplateName"));
                xmlString.AppendLine("<ProjectReportsEnabled>" + createReportsSite.ToString().ToLower() + "</ProjectReportsEnabled>");
                xmlString.AppendLine("<ProjectReportsForceUpload>" + createReportsSite.ToString().ToLower() + "</ProjectReportsForceUpload>");
                xmlString.AppendLine(this.EncodeForXml("sites/" + this._tfsTeamProjectCollection.Name + "/" + teamProjectName, "ProjectSitePath"));
                xmlString.AppendLine("</Project>");

                string fileName = logPath + "\\" + Guid.NewGuid().ToString("N");

                File.WriteAllText(fileName, xmlString.ToString());

                var dte = Activator.CreateInstance(Type.GetTypeFromProgID("VisualStudio.DTE.12.0", true)) as DTE2;
                if (dte == null)
                {
                    throw new Exception("Visual Studio 2013 IDE doesn't exist on the current machine");
                }
                else
                {
                    dte.SuppressUI = true;
                    dte.ExecuteCommand("File.BatchNewTeamProject", fileName);
                    dte.Quit();
                }

                if (File.Exists(logPath + teamProjectName + ".log"))
                {
                    logOutput = File.ReadAllText(logPath + teamProjectName + ".log");
                }

                result = logOutput.Contains("Team Project Batch Creation succeeded");
            }
            else
            {
                throw new Exception("The project " + teamProjectName + " already exists in Team Foundation Server Collection " + this._tfsCollectionUri + ".");
            }

            return result;
        }

        /// <summary>
        ///     Deletes the team project.
        /// </summary>
        /// <param name="teamProjectName">Name of the team project.</param>
        /// <param name="force">Force project deletion.</param>
        /// <param name="excludeWss">Specifies to not delete the SharePoint site that is associated with the team project. Specify this option to maintain the existing site so that other team projects can continue using it.</param>
        /// <exception cref="System.ArgumentException">
        ///     No team project name was specified;teamProjectName
        ///     or
        ///     The project name ( + teamProjectName + ) doesn't exists.;teamProjectName
        /// </exception>
        public bool DeleteTeamProject(string teamProjectName, bool force = false, bool excludeWss = true)
        {
            List<Exception> exceptions;
            return this.DeleteTeamProject(teamProjectName, out exceptions, force, excludeWss);
        }

        /// <summary>
        ///     Deletes the team project.
        /// </summary>
        /// <param name="teamProjectName">Name of the team project.</param>
        /// <param name="force">Force project deletion.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <param name="excludeWss">Specifies to not delete the SharePoint site that is associated with the team project. Specify this option to maintain the existing site so that other team projects can continue using it.</param>
        /// <exception cref="System.ArgumentException">
        ///     No team project name was specified;teamProjectName
        ///     or
        ///     The project name ( + teamProjectName + ) doesn't exists.;teamProjectName
        /// </exception>
        public bool DeleteTeamProject(string teamProjectName, out List<Exception> exceptions, bool force = false, bool excludeWss = true)
        {
            if (string.IsNullOrEmpty(teamProjectName))
            {
                throw new ArgumentException("No team project name was specified", "teamProjectName");
            }
            else
            {
                bool foundProjectName = false;
                foreach (string projectname in this.ListTeamProjectNames())
                {
                    if (string.Compare(projectname, teamProjectName, true) == 0)
                    {
                        foundProjectName = true;
                        break;
                    }
                }

                if (!foundProjectName)
                {
                    throw new ArgumentException("The project name (" + teamProjectName + ") doesn't exists.", "teamProjectName");
                }
            }

            Microsoft.TeamFoundation.Client.TeamProjectDeleter teamProjectDeleter = new TeamProjectDeleter(this._tfsTeamProjectCollection, teamProjectName, force);
            teamProjectDeleter.ExcludeWss = excludeWss;
            teamProjectDeleter.Delete();
            exceptions = teamProjectDeleter.IssuesDeletingProject;
            return teamProjectDeleter.IssuesDeletingProject == null || teamProjectDeleter.IssuesDeletingProject.Count == 0;
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
        ///     Gets the team project share point URI.
        /// </summary>
        /// <param name="teamProjectName">Name of the team project.</param>
        /// <returns></returns>
        public Uri GetTeamProjectSharepointUri(string teamProjectName)
        {
            RegistrationEntry[] entries = this._registration.GetRegistrationEntries("TeamProjects");
            ServiceInterface endpoint = entries[0].ServiceInterfaces.FirstOrDefault(si => si.Name == teamProjectName + ":Portal");
            if (endpoint == null)
            {
                return null;
            }

            return new Uri(endpoint.Url);
        }

        /// <summary>
        ///     Lists the team project names.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ListTeamProjectNames()
        {
            return this._commonStructureService.ListAllProjects().Select(o => o.Name);
        }

        /// <summary>
        ///     Checks if the team project name exists.
        /// </summary>
        /// <param name="teamProjectName">Name of the team project.</param>
        /// <returns></returns>
        public bool TeamProjectExists(string teamProjectName)
        {
            return (from o in this.ListTeamProjectNames()
                    where string.Compare(o, teamProjectName, true) == 0
                    select o).FirstOrDefault() != null;
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
                // TODO: Managed cleanup code here, while managed refs still valid
            }

            // TODO: Unmanaged cleanup code here
            this.disposed = true;
        }

        private string EncodeForXml(string input, string tag)
        {
            var doc = new XmlDocument();
            XmlElement element = doc.CreateElement(tag);
            element.InnerText = input;
            return element.OuterXml;
        }

        #endregion
    }
}