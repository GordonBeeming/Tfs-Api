namespace TfsApi.Administration.Dto
{
    #region

    using System;
    using System.Collections.Generic;

    #endregion

    public class ProjectIteration
    {
        #region Public Properties

        public List<ProjectIteration> Children { get; set; }

        public DateTime? FinishDate { get; set; }

        public string FullPath { get; set; }

        public string Name { get; set; }

        public ProjectIteration ParentProjectIteration { get; set; }

        public string ParentUri { get; set; }

        public string Path { get; set; }

        public string ProjectUri { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public DateTime? StartDate { get; set; }

        public string StructureType { get; set; }

        public string Uri { get; set; }

        #endregion
    }
}