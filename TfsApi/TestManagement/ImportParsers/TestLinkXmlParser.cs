namespace TfsApi.TestManagement.ImportParsers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using TfsApi.TestManagement.Contracts;

    #endregion

    public static class TestLinkXmlParser
    {
        public static List<ITestSuite> LoadTestSuites(string xml)
        {
            List<ITestSuite> result;

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            result = GetTestSuitesFrom(doc.SelectNodes("/testsuite"));

            return result;
        }

        private static List<ITestSuite> GetTestSuitesFrom(XmlNodeList nodes)
        {
            var result = new List<ITestSuite>();

            foreach (XmlNode testSuiteNode in nodes)
            {
                string nameOriginal = GetAttributeValueAsString(testSuiteNode, "name");
                ITestSuite testSuite = result.Where(o => string.Compare(o.Name, nameOriginal, System.StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();

                if (testSuite == null)
                {
                    testSuite = TestSuiteFactory.CreateDefaultTestSuite();
                    string name = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(nameOriginal));
                    if (nameOriginal != name)
                    {
                        testSuite.Name = name + " - " + Guid.NewGuid().ToString("N");
                    }
                    else
                    {
                        testSuite.Name = name;
                    }

                    testSuite.NodeCount = SelectAndReadNodeTextAsInt32(testSuiteNode, "node_order");
                    testSuite.Description = SelectAndReadNodeTextAsString(testSuiteNode, "details");

                    testSuite.TestCases = GetTestCasesFrom(testSuiteNode);

                    testSuite.TestSuites = GetTestSuitesFrom(testSuiteNode.SelectNodes("testsuite"));
                    result.Add(testSuite);
                }
            }

            return result;
        }

        private static List<ITestCase> GetTestCasesFrom(XmlNode testSuiteNode)
        {
            var result = new List<ITestCase>();

            XmlNodeList xmlNodeList = testSuiteNode.SelectNodes("testcase");
            if (xmlNodeList != null)
            {
                foreach (XmlNode testCaseNode in xmlNodeList)
                {
                    string nameOriginal = GetAttributeValueAsString(testCaseNode, "name");
                    ITestCase testCase = result.Where(o => string.Compare(o.Name, nameOriginal, System.StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();

                    if (testCase == null)
                    {
                        string name = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(GetAttributeValueAsString(testCaseNode, "name")));
                        testCase = TestCaseFactory.CreateDefaultTestCase();
                        if (nameOriginal != name)
                        {
                            testCase.Name = name + " - " + Guid.NewGuid().ToString("N");
                        }
                        else
                        {
                            testCase.Name = name;
                        }

                        testCase.InternalID = GetAttributeValueAsInt32(testCaseNode, "internalid");
                        testCase.NodeCount = SelectAndReadNodeTextAsInt32(testCaseNode, "node_order");
                        testCase.ExternalID = SelectAndReadNodeTextAsInt32(testCaseNode, "externalid");
                        testCase.LinkInTestLink = "http://<Server><Virtual Directory>/linkto.php?tprojectPrefix=C&item=testcase&id=C-" + testCase.ExternalID;
                        testCase.Version = SelectAndReadNodeTextAsInt32(testCaseNode, "version");
                        testCase.Description = SelectAndReadNodeTextAsString(testCaseNode, "summary");
                        testCase.PreConditions = SelectAndReadNodeTextAsString(testCaseNode, "preconditions");
                        testCase.ExecutionType = SelectAndReadNodeTextAsInt32(testCaseNode, "execution_type");
                        testCase.Importance = SelectAndReadNodeTextAsInt32(testCaseNode, "importance");
                        testCase.Steps = GetTestCaseStepsFrom(testCaseNode.SelectSingleNode("steps"));
                        result.Add(testCase);
                    }
                }
            }


            return result;
        }

        private static List<ITestCaseStep> GetTestCaseStepsFrom(XmlNode testStepsNode)
        {
            var result = new List<ITestCaseStep>();

            if (testStepsNode != null)
            {
                XmlNodeList xmlNodeList = testStepsNode.SelectNodes("step");
                if (xmlNodeList != null)
                {
                    foreach (XmlNode stepNode in xmlNodeList)
                    {
                        ITestCaseStep testCaseStep = TestCaseStepFactory.CreateDefaultTestCaseStep();
                        testCaseStep.StepNumber = SelectAndReadNodeTextAsInt32(stepNode, "step_number");
                        testCaseStep.Actions = SelectAndReadNodeTextAsString(stepNode, "actions");
                        testCaseStep.ExpectedResults = SelectAndReadNodeTextAsString(stepNode, "expectedresults");
                        testCaseStep.ExecutionType = SelectAndReadNodeTextAsInt32(stepNode, "execution_type");
                        result.Add(testCaseStep);
                    }
                }
            }

            return result;
        }

        private static int GetAttributeValueAsInt32(XmlNode node, string attributeName)
        {
            int value = -1;
            string valueAsString = GetAttributeValueAsString(node, attributeName);
            int.TryParse(valueAsString, out value);
            return value;
        }

        private static string GetAttributeValueAsString(XmlNode testSuiteNode, string attributeName)
        {
            string value = string.Empty;
            if (testSuiteNode.Attributes != null && testSuiteNode.Attributes[attributeName] != null)
            {
                value = testSuiteNode.Attributes[attributeName].Value.Trim();
            }

            return value;
        }

        private static int SelectAndReadNodeTextAsInt32(XmlNode testSuiteNode, string nodeName)
        {
            int value = -1;
            if (testSuiteNode.SelectNodes(nodeName) != null)
            {
                XmlNode xmlNode = testSuiteNode.SelectSingleNode(nodeName);
                if (xmlNode != null)
                {
                    value = ReadNodeTextAsInt32(xmlNode);
                }
            }

            return value;
        }

        private static string SelectAndReadNodeTextAsString(XmlNode testSuiteNode, string nodeName)
        {
            string value = string.Empty;
            if (testSuiteNode.SelectNodes(nodeName) != null)
            {
                XmlNode xmlNode = testSuiteNode.SelectSingleNode(nodeName);
                if (xmlNode != null)
                {
                    value = xmlNode.InnerText.Trim();
                }
            }

            return value;
        }

        private static int ReadNodeTextAsInt32(XmlNode xmlNode)
        {
            int value = -1;
            int.TryParse(xmlNode.InnerText.Trim(), out value);
            return value;
        }
    }
}