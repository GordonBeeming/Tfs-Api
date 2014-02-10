using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TfsApi.Administration.Contracts;
using TfsApi.Administration.Dto;
using TfsApi.Contracts;

namespace TfsApi.Administration.Workers
{
    internal class TfsTeam : ITfsTeam
    {
        private readonly ITfsCredentials tfsCredentials;

        public TfsTeam(ITfsCredentials credentials)
        {
            tfsCredentials = credentials;
        }

        // Summary:
        //     Indicates whether the team is the "default team" for the team project
        //
        // Returns:
        //     Returns System.Boolean.        
        public bool IsDefaultTeam { get; internal set; }

        //
        // Summary:
        //     The Project Uri of the team project which the team belongs to
        //
        // Returns:
        //     Returns System.String.
        public string ProjectUri { get; internal set; }

        //
        // Summary:
        //     The identifier for the team
        //
        // Returns:
        //     Returns System.Guid.
        public Guid TeamId { get; internal set; }

        //
        // Summary:
        //     The team name
        //
        // Returns:
        //     Returns System.String.
        public string TeamName { get; internal set; }

        //
        // Summary:
        //     The team settings
        //
        // Returns:
        //     Returns Microsoft.TeamFoundation.ProcessConfiguration.Client.TeamSettings.
        public TeamSettings TeamSettings { get; internal set; }

        public TeamConfiguration TeamConfiguration { get; internal set; }

        public TfsTeamService TeamService { get; internal set; }

        public ProjectDetail ProjectDetails { get; internal set; }

        private List<ProjectIteration> iterationPaths = null;

        public List<ProjectIteration> IterationPaths
        {
            get
            {
                if (iterationPaths == null)
                {
                    LoadIterationPaths();
                }
                return iterationPaths;
            }
            internal set { iterationPaths = value; }
        }

        private List<ProjectArea> areaPaths = null;

        public List<ProjectArea> AreaPaths
        {
            get
            {
                if (areaPaths == null)
                {
                    LoadAreaPaths();
                }
                return areaPaths;
            }
            internal set { areaPaths = value; }
        }

        private Microsoft.TeamFoundation.Client.TeamFoundationTeam teamFoundationTeam = null;
        public Microsoft.TeamFoundation.Client.TeamFoundationTeam TeamFoundationTeam
        {
            get
            {
                if (teamFoundationTeam == null && TeamService != null)
                {
                    teamFoundationTeam = TeamService.ReadTeam(ProjectUri, TeamName, null);
                }
                return teamFoundationTeam;
            }
        }

        public TeamSettingsConfigurationService TeamSettingsConfigurationService { get; internal set; }


        private string FormatPath(string path)
        {
            if (path == null)
            {
                return string.Empty;
            }
            if (path.StartsWith(this.ProjectDetails.ProjectName + "\\"))
            {
                return path;
            }
            return string.Format(@"{0}\{1}", this.ProjectDetails.ProjectName, path.TrimStart('\\'));
        }



        public bool IsIterationPathEnabled(string iterationPath)
        {
            if (IterationPaths == null)
            {
                LoadIterationPaths();
            }
            string iterationFullPath = FormatPath(iterationPath);
            return IterationPaths.Exists(o => string.Compare(o.FullPath, iterationFullPath, true) == 0);
        }

        private void LoadIterationPaths()
        {
            IterationPaths = new List<ProjectIteration>();
            IIterationManager iterationManager = IterationManagerFactory.GetManager(ProjectDetails, tfsCredentials);
            IterationPaths.AddRange(iterationManager.FlattenIterations(iterationManager.ListIterations()).Where(o => TeamConfiguration.TeamSettings.IterationPaths.Contains(o.FullPath)));
        }

        private void LoadAreaPaths()
        {
            AreaPaths = new List<ProjectArea>();
            IAreaManager areaManager = AreaManagerFactory.GetManager(ProjectDetails, tfsCredentials);
            AreaPaths.AddRange(areaManager.FlattenAreas(areaManager.ListAreas()).Where(o => ReadTeamAreaPaths().Contains(o.FullPath)));
        }

        private string[] ReadTeamAreaPaths()
        {
            List<string> areaPaths = new List<string>();
            foreach (TeamFieldValue item in TeamConfiguration.TeamSettings.TeamFieldValues)
            {
                areaPaths.Add(item.Value);
            }
            return areaPaths.ToArray();
        }


        public void EnableIterationPath(string iterationPath)
        {
            DefaultTeamSettings();

            var iterations = new string[TeamConfiguration.TeamSettings.IterationPaths.Length + 1];
            TeamConfiguration.TeamSettings.IterationPaths.CopyTo(iterations, 0);
            iterations[iterations.Length - 1] = this.FormatPath(iterationPath);
            TeamConfiguration.TeamSettings.IterationPaths = iterations;

            this.TeamSettingsConfigurationService.SetTeamSettings(this.TeamConfiguration.TeamId, TeamConfiguration.TeamSettings);

            LoadIterationPaths();
        }


        public void DisableIterationPath(string iterationPath)
        {
            DefaultTeamSettings();

            iterationPath = this.FormatPath(iterationPath);
            var iterations = TeamConfiguration.TeamSettings.IterationPaths.Where(o => string.Compare(o, iterationPath, true) != 0).ToArray();
            if (TeamConfiguration.TeamSettings.TeamFieldValues.Length == iterations.Length)
            {
                throw new Exception("The iteration path '" + iterationPath + "' is not enabled or doesn't exist of the current team.");
            }
            TeamConfiguration.TeamSettings.IterationPaths = iterations;

            this.TeamSettingsConfigurationService.SetTeamSettings(this.TeamConfiguration.TeamId, TeamConfiguration.TeamSettings);

            LoadIterationPaths();
        }
        
        public void EnableAreaPath(string areaPath, bool includeChildren)
        {
            DefaultTeamSettings();

            var areas = new TeamFieldValue[TeamConfiguration.TeamSettings.TeamFieldValues.Length + 1];
            TeamConfiguration.TeamSettings.TeamFieldValues.CopyTo(areas, 0);
            areas[areas.Length - 1] = new TeamFieldValue { Value = this.FormatPath(areaPath), IncludeChildren = includeChildren };
            TeamConfiguration.TeamSettings.TeamFieldValues = areas;
            this.TeamSettingsConfigurationService.SetTeamSettings(this.TeamConfiguration.TeamId, TeamConfiguration.TeamSettings);

            LoadAreaPaths();
        }

        public void SwitchTeamEnabledAreaPaths(Dictionary<string, bool> areaPathsWithIncludeChildren)
        {
            DefaultTeamSettings();

            TeamConfiguration.TeamSettings.TeamFieldValues = areaPathsWithIncludeChildren.Select(o => new TeamFieldValue { Value = this.FormatPath(o.Key), IncludeChildren = o.Value }).ToArray();

            this.TeamSettingsConfigurationService.SetTeamSettings(this.TeamConfiguration.TeamId, TeamConfiguration.TeamSettings);

            LoadAreaPaths();
        }


        public void DisableAreaPath(string areaPath)
        {
            DefaultTeamSettings();
            areaPath = this.FormatPath(areaPath);
            TeamFieldValue[] areas = TeamConfiguration.TeamSettings.TeamFieldValues.Where(o => string.Compare(o.Value, areaPath, true) != 0).ToArray();
            if (TeamConfiguration.TeamSettings.TeamFieldValues.Length == areas.Length)
            {
                throw new Exception("The area path '" + areaPath + "' is not enabled or doesn't exist of the current team.");
            }
            TeamConfiguration.TeamSettings.TeamFieldValues = areas;
            this.TeamSettingsConfigurationService.SetTeamSettings(this.TeamConfiguration.TeamId, TeamConfiguration.TeamSettings);

            LoadAreaPaths();
        }

        private void DefaultTeamSettings()
        {
            if (TeamConfiguration.TeamSettings.BacklogIterationPath == null)
            {
                TeamConfiguration.TeamSettings.BacklogIterationPath = ProjectDetails.ProjectName;
            }
            //if (TeamConfiguration.TeamSettings.CurrentIterationPath == null && TeamConfiguration.TeamSettings.IterationPaths.Length == 0)
            //{
            //    TeamConfiguration.TeamSettings.IterationPaths = new[] { ProjectDetails.ProjectName };
            //}
            if (TeamConfiguration.TeamSettings.TeamFieldValues == null || TeamConfiguration.TeamSettings.TeamFieldValues.Length == 0)
            {
                TeamConfiguration.TeamSettings.TeamFieldValues = new TeamFieldValue[] { new TeamFieldValue { IncludeChildren = true, Value = this.ProjectDetails.ProjectName } };
            }
        }


        public bool IsAreaPathEnabled(string areaName)
        {
            if (AreaPaths == null || AreaPaths.Count == 0)
            {
                LoadAreaPaths();
            }
            string areaFullPath = FormatPath(areaName);
            return AreaPaths.Exists(o => string.Compare(o.FullPath, areaFullPath, true) == 0);
        }
    }
}
