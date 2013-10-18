using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TfsApi.TestManagement.Contracts
{
    public interface ITestCase
    {
        int InternalID { get; set; }

        string Name { get; set; }

        int NodeCount { get; set; }

        int ExternalID { get; set; }

        int Version { get; set; }

        string Description { get; set; }

        string PreConditions { get; set; }

        int ExecutionType { get; set; }

        int Importance { get; set; }

        string LinkInTestLink { get; set; }

        List<ITestCaseStep> Steps { get; set; }
    }
}
