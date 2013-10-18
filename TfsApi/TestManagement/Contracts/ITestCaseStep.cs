namespace TfsApi.TestManagement.Contracts
{
    public interface ITestCaseStep
    {
        int StepNumber { get; set; }

        string Actions { get; set; }

        string ExpectedResults { get; set; }

        int ExecutionType { get; set; }
    }
}