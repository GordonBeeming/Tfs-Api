namespace TfsApi.Administration.Contracts
{
    #region

    using System;
    using System.Collections.Generic;

    using TfsApi.Administration.Dto;

    #endregion

    public interface IIterationManager : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Adds the new iteration.
        /// </summary>
        /// <param name="newIterationName">New name of the iteration.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="finishDate">The finish date.</param>
        /// <param name="enableforTfsTeams">Enable this new iteration path for all the supplied teams.</param>
        void AddNewIteration(string newIterationName, DateTime? startDate = null, DateTime? finishDate = null, List<ITfsTeam> enableforTfsTeams = null, bool refreshCache = false);

        /// <summary>
        ///     Checks if path already exists.
        /// </summary>
        /// <param name="iterationPath">The iteration path.</param>
        /// <returns></returns>
        bool CheckIfPathAlreadyExists(string iterationPath);

        /// <summary>
        ///     Deletes the iteration.
        /// </summary>
        /// <param name="projectIteration">The project iteration.</param>
        void DeleteIteration(ProjectIteration projectIteration);

        /// <summary>
        ///     Deletes the iteration using iteration path.
        /// </summary>
        /// <param name="iterationPath">The iteration path.</param>
        void DeleteIterationUsingIterationPath(string iterationPath);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void Dispose();

        /// <summary>
        ///     Flattens the iterations.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        List<ProjectIteration> FlattenIterations(List<ProjectIteration> list);

        /// <summary>
        ///     Determines whether [is iteration path visible for iteration planning] [the specified iteration path].
        /// </summary>
        /// <param name="tfsTeam">The TFS Team.</param>
        /// <param name="iterationPath">The iteration path.</param>
        /// <returns>
        ///     <c>true</c> if [is iteration path visible for iteration planning] [the specified iteration path]; otherwise, <c>false</c>.
        /// </returns>
        bool IsIterationPathEnabled(ITfsTeam tfsTeam, string iterationPath);

        /// <summary>
        ///     Lists the Iterations.
        /// </summary>
        /// <returns></returns>
        List<ProjectIteration> ListIterations();

        ProjectIteration FindProjectIteration(string fullIterationPath);

        void EnableIterationPath(ITfsTeam tfsTeam, string iterationName, bool includeChildren);

        void DisableIterationPath(ITfsTeam tfsTeam, string iterationName);

        #endregion
    }
}