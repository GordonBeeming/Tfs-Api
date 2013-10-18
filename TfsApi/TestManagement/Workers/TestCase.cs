namespace TfsApi.TestManagement.Workers
{
    using System.Collections.Generic;
    using TfsApi.TestManagement.Contracts;

    internal class TestCase : ITestCase
    {
        public int InternalID { get; set; }

        public string Name { get; set; }

        public int NodeCount { get; set; }

        public int ExternalID { get; set; }

        public int Version { get; set; }

        public string Description { get; set; }

        public string PreConditions { get; set; }

        public int ExecutionType { get; set; }

        public int Importance { get; set; }

        public string LinkInTestLink { get; set; }

        public List<ITestCaseStep> Steps { get; set; }
    }
}