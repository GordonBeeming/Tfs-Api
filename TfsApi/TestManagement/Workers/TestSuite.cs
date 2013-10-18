namespace TfsApi.TestManagement.Workers
{
    using System.Collections.Generic;
    using TfsApi.TestManagement.Contracts;

    internal class TestSuite : ITestSuite
    {
        public string Name { get; set; }

        public List<ITestSuite> TestSuites { get; set; }

        public int NodeCount { get; set; }

        public string Description { get; set; }

        public List<ITestCase> TestCases { get; set; }
    }
}