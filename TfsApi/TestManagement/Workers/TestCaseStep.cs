using TfsApi.TestManagement.Contracts;
namespace TfsApi.TestManagement.Workers
{
    internal class TestCaseStep : ITestCaseStep
    {
        public int StepNumber { get; set; }

        public string Actions { get; set; }

        public string ExpectedResults { get; set; }

        public int ExecutionType { get; set; }
    }
}