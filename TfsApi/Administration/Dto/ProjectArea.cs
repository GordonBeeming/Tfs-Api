using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TfsApi.Administration.Dto
{
    public class ProjectArea
    {
        public string Name { get; set; }

        public List<ProjectArea> Children { get; set; }

        public bool Enabled { get; set; }

        public string FullPath { get; set; }

        public ProjectArea ParentProjectArea { get; set; }

        public string ParentUri { get; set; }

        public string Path { get; set; }

        public string ProjectUri { get; set; }

        public string StructureType { get; set; }

        public string Uri { get; set; }
    }
}
