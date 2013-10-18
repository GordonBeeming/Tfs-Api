using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TfsApi.TestManagement.Contracts
{
    public interface ITestSuite
    {
        string Name { get; set; }

        List<ITestSuite> TestSuites { get; set; }

        int NodeCount { get; set; }

        string Description { get; set; }

        List<ITestCase> TestCases { get; set; }
    }
}
