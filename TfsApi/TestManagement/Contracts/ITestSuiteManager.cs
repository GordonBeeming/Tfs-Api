using Microsoft.TeamFoundation.TestManagement.Client;
using System.Collections.Generic;
using System.IO;
using TfsApi.TestManagement.DTO;
namespace TfsApi.TestManagement.Contracts
{
    public interface ITestSuiteManager
    {
        List<ITestSuite> LoadTestSuitesFromTestLinkExportFile(System.IO.FileInfo fi);

        List<ITestSuite> LoadTestSuitesFromTestLinkExportFileContents(string fileContents);

        List<TestCaseConflict> CreateTestSuites(List<ITestSuite> testSuites, ITestManagementTeamProject selectedProject, IStaticTestSuite staticTestSuite = null, bool throwExceptionIfTestCaseConflicts = true);
    }
}