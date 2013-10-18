using TfsApi.TestManagement.DTO;
namespace TfsApi.TestManagement.Workers
{
    #region

    using Microsoft.TeamFoundation.TestManagement.Client;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using TfsApi.Contracts;
    using TfsApi.TestManagement.Contracts;
    using TfsApi.TestManagement.ImportParsers;
    using TfsApi.Utilities;

    using ITestCase = TfsApi.TestManagement.Contracts.ITestCase;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using TfsApi.TestManagement.Exceptions;

    #endregion

    internal class TestSuiteManager : ITestSuiteManager
    {
        #region Fields

        private readonly Uri _tfsCollectionUri;

        private readonly ITfsCredentials _tfsCredentials;

        #endregion

        #region Constructors and Destructors

        public TestSuiteManager(Uri tfsCollectionUri, ITfsCredentials tfsCredentials)
        {
            // TODO: Complete member initialization
            this._tfsCollectionUri = tfsCollectionUri;
            this._tfsCredentials = tfsCredentials;
        }

        #endregion

        public List<ITestSuite> LoadTestSuitesFromTestLinkExportFile(System.IO.FileInfo fi)
        {
            string fileContents = System.Text.ASCIIEncoding.ASCII.GetString(StreamUtility.ReadByteArrayFromFilename(fi.Name));

            return LoadTestSuitesFromTestLinkExportFileContents(fileContents);
        }

        public List<ITestSuite> LoadTestSuitesFromTestLinkExportFileContents(string fileContents)
        {
            return TestLinkXmlParser.LoadTestSuites(fileContents);
        }

        public List<TestCaseConflict> CreateTestSuites(List<ITestSuite> testSuites, ITestManagementTeamProject selectedProject, IStaticTestSuite staticTestSuite = null, bool throwExceptionIfTestCaseConflicts = true)
        {
            List<TestCaseConflict> result = new List<TestCaseConflict>();

            ITestSuiteEntryCollection suitcollection = staticTestSuite.Entries;
            foreach (TestSuite testSuite in testSuites)
            {
                if (string.IsNullOrEmpty(testSuite.Name))
                {
                    this.CreateTestSuites(testSuite.TestSuites, selectedProject, staticTestSuite, throwExceptionIfTestCaseConflicts);
                }
                else
                {
                    ITestSuiteEntry obj = (from o in suitcollection where string.Compare(o.Title, testSuite.Name, true) == 0 select o).FirstOrDefault() as ITestSuiteEntry;
                    IStaticTestSuite newSuite = (IStaticTestSuite)obj.TestSuite;

                    if (newSuite == null)
                    {
                        newSuite = selectedProject.TestSuites.CreateStatic();
                        newSuite.Title = testSuite.Name;
                        newSuite.Description = testSuite.Description;

                        suitcollection.Add(newSuite);
                    }

                    result.AddRange(this.CreateTestSuites(testSuite.TestSuites, selectedProject, newSuite, throwExceptionIfTestCaseConflicts));

                    result.AddRange(this.CreateTestCases(selectedProject, testSuite, newSuite, throwExceptionIfTestCaseConflicts));
                }
            }

            return result;
        }

        private List<TestCaseConflict> CreateTestCases(ITestManagementTeamProject selectedProject, TestSuite testSuite, IStaticTestSuite newSuite, bool throwExceptionIfTestCaseConflicts)
        {
            List<TestCaseConflict> result = new List<TestCaseConflict>();

            ITestSuiteEntryCollection suitcollection = newSuite.Entries;
            foreach (ITestCase testCase in testSuite.TestCases)
            {
                ITestSuiteEntry obj = (from o in suitcollection where string.Compare(o.Title, testCase.Name, true) == 0 select o).FirstOrDefault() as ITestSuiteEntry;
                Microsoft.TeamFoundation.TestManagement.Client.ITestCase newTestCase = obj.TestCase;

                if (newTestCase == null)
                {
                    newTestCase = selectedProject.TestCases.Create();
                    newTestCase.Title = testCase.Name;
                    newTestCase.Description = "<p><strong>Summary</strong></p>" + testCase.Description + "<p><strong>Pre Conditions</strong></p>" + testCase.PreConditions;
                    var link = new Hyperlink(testCase.LinkInTestLink);
                    newTestCase.Links.Add(link);
                    newTestCase.Priority = testCase.Importance;
                    newTestCase.CustomFields["Assigned To"].Value = string.Empty;

                    LoadTestSteps(testCase, newTestCase);

                    newTestCase.Save();

                    suitcollection.Add(newTestCase);
                }
                else
                {
                    if (newTestCase.Actions.Count != testCase.Steps.Count)
                    {
                        TestCaseConflict tcc = new TestCaseConflict();
                        tcc.Message = testCase.Name + " (-:-) already exists and has a different step count" + Environment.NewLine + "(" + testCase.LinkInTestLink + ")";
                        tcc.TestCase = testCase;
                        tcc.TfsTestCase = newTestCase;
                        if (throwExceptionIfTestCaseConflicts)
                        {
                            throw new TestCaseConflictException(tcc);
                        }
                        result.Add(tcc);
                    }
                    else
                    {
                        string differntFields = string.Empty;
                        for (int pos = 0; pos < testCase.Steps.Count; pos++)
                        {
                            if (HtmlRemovalUtility.StripTagsCharArrayWithExtraParsing(((ITestStep)newTestCase.Actions[pos]).Description) != HtmlRemovalUtility.StripTagsCharArrayWithExtraParsing(testCase.Steps[pos].Actions))
                            {
                                differntFields += "\t[" + pos + "] Description : " + testCase.Steps[pos].Actions + Environment.NewLine;
                            }
                            if (HtmlRemovalUtility.StripTagsCharArrayWithExtraParsing(((ITestStep)newTestCase.Actions[pos]).ExpectedResult) != HtmlRemovalUtility.StripTagsCharArrayWithExtraParsing(testCase.Steps[pos].ExpectedResults))
                            {
                                differntFields += "\t[" + pos + "] ExpectedResults : " + testCase.Steps[pos].ExpectedResults + Environment.NewLine;
                            }
                        }
                        if (differntFields.Length > 0)
                        {
                            TestCaseConflict tcc = new TestCaseConflict();
                            tcc.Message = testCase.Name + " (-:-) already exists and is different" + Environment.NewLine + "(" + testCase.LinkInTestLink + ")" + Environment.NewLine + differntFields;
                            tcc.TestCase = testCase;
                            tcc.TfsTestCase = newTestCase;
                            if (throwExceptionIfTestCaseConflicts)
                            {
                                throw new TestCaseConflictException(tcc);
                            }
                            result.Add(tcc);
                        }
                    }
                }
            }

            return result;
        }

        private static void LoadTestSteps(ITestCase testCase, Microsoft.TeamFoundation.TestManagement.Client.ITestCase newTestCase)
        {
            foreach (ITestCaseStep step in testCase.Steps)
            {
                ITestStep testStep = newTestCase.CreateTestStep();
                testStep.Title = step.Actions;
                testStep.Description = step.Actions;
                testStep.ExpectedResult = step.ExpectedResults;
                testStep.TestStepType = string.IsNullOrEmpty(testStep.ExpectedResult) ? TestStepType.ActionStep : TestStepType.ValidateStep;
                newTestCase.Actions.Add(testStep);
            }
        }

    }
}