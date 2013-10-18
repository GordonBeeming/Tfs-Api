using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TfsApi.Administration.Dto;

namespace TfsApi.Administration.Contracts
{
    public interface IAreaManager : IDisposable
    {
        List<ProjectArea> FlattenAreas(List<ProjectArea> list);

        List<ProjectArea> ListAreas();

        void AddNewArea(string areaPath, List<ITfsTeam> enableforTfsTeams = null, bool includeChildren = true, bool refreshCache = false);

        void AddNewArea(string areaPath, Dictionary<ITfsTeam, bool> enableforTfsTeams, bool refreshCache = false);

        bool CheckIfPathAlreadyExists(string newAreaName);

        bool IsAreaPathEnabled(ITfsTeam tfsTeam, string areaName);

        void DeleteArea(ProjectArea projectArea);

        void DeleteAreaUsingAreaPath(string areaPath);

        ProjectArea FindProjectArea(string fullAreaPath);

        void EnableAreaPath(ITfsTeam tfsTeam, string areaName, bool includeChildren);

        void DisableAreaPath(ITfsTeam tfsTeam, string areaName);
    }
}
