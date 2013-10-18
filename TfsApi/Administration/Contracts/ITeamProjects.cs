// // <copyright file="ITeamProjects.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the ITeamProjects.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Administration.Contracts
{
    #region

    using System;
    using System.Collections.Generic;

    #endregion

    public interface ITeamProjects : IDisposable
    {
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
        bool CreateTeamProject(string teamProjectName, string teamProjectDescription, string processTemplate, bool createSharepointSite, bool createReportsSite);

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
        bool CreateTeamProject(string teamProjectName, string teamProjectDescription, string processTemplate, bool createSharepointSite, bool createReportsSite, ref string logOutput);

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
        bool DeleteTeamProject(string teamProjectName, bool force = false, bool excludeWss = true);

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
        bool DeleteTeamProject(string teamProjectName, out List<Exception> exceptions, bool force = false, bool excludeWss = true);

        /// <summary>
        ///     Gets the team project share point URI.
        /// </summary>
        /// <param name="teamProjectName">Name of the team project.</param>
        /// <returns></returns>
        Uri GetTeamProjectSharepointUri(string teamProjectName);

        /// <summary>
        ///     Lists the team project names.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> ListTeamProjectNames();

        /// <summary>
        ///     Checks if the team project name exists.
        /// </summary>
        /// <param name="teamProjectName">Name of the team project.</param>
        /// <returns></returns>
        bool TeamProjectExists(string teamProjectName);

        #endregion
    }
}