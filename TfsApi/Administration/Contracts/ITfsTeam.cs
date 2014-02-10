using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TfsApi.Administration.Dto;

namespace TfsApi.Administration.Contracts
{
    public interface ITfsTeam
    {
        // Summary:
        //     Indicates whether the team is the "default team" for the team project
        //
        // Returns:
        //     Returns System.Boolean.        
        bool IsDefaultTeam { get; }

        //
        // Summary:
        //     The Project Uri of the team project which the team belongs to
        //
        // Returns:
        //     Returns System.String.
        string ProjectUri { get; }

        //
        // Summary:
        //     The identifier for the team
        //
        // Returns:
        //     Returns System.Guid.
        Guid TeamId { get; }

        //
        // Summary:
        //     The team name
        //
        // Returns:
        //     Returns System.String.
        string TeamName { get; }

        ProjectDetail ProjectDetails { get; }


        //
        // Summary:
        //     The team settings
        //
        // Returns:
        //     Returns Microsoft.TeamFoundation.ProcessConfiguration.Client.TeamSettings.
        TeamSettings TeamSettings { get; }

        TeamFoundationTeam TeamFoundationTeam { get; }

        TeamConfiguration TeamConfiguration { get; }

        TfsTeamService TeamService { get; }

        void EnableIterationPath(string iterationPath);

        void DisableIterationPath(string iterationPath);

        bool IsIterationPathEnabled(string iterationPath);

        List<ProjectIteration> IterationPaths { get; }

        List<ProjectArea> AreaPaths { get; }

        void EnableAreaPath(string areaPath, bool includeChildren);

        void SwitchTeamEnabledAreaPaths(Dictionary<string, bool> areaPathsWithIncludeChildren);

        void DisableAreaPath(string areaPath);

        bool IsAreaPathEnabled(string areaName);
    }
}
