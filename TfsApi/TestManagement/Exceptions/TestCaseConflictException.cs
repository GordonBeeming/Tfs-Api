using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsApi.TestManagement.DTO;

namespace TfsApi.TestManagement.Exceptions
{
    public class TestCaseConflictException : Exception
    {
        public Microsoft.TeamFoundation.TestManagement.Client.ITestCase TfsTestCase
        {
            get
            {
                return Conflict.TfsTestCase;
            }
        }

        public TfsApi.TestManagement.Contracts.ITestCase TestCase
        {
            get
            {
                return Conflict.TestCase;
            }
        }

        public TestCaseConflict Conflict;

        public TestCaseConflictException(string message,
                                            Microsoft.TeamFoundation.TestManagement.Client.ITestCase tfsTestCase,
                                            TfsApi.TestManagement.Contracts.ITestCase testCase)
            : base(message)
        {
            Conflict = new TestCaseConflict();
            Conflict.Message = message;
            Conflict.TestCase = testCase;
            Conflict.TfsTestCase = tfsTestCase;
        }

        public TestCaseConflictException(TestCaseConflict conflict)
            : base(conflict.Message)
        {
            Conflict = conflict;
        }

        public TestCaseConflictException(string message,
                                            Microsoft.TeamFoundation.TestManagement.Client.ITestCase tfsTestCase,
                                            TfsApi.TestManagement.Contracts.ITestCase testCase,
                                            Exception innerException)
            : base(message, innerException)
        {
            Conflict = new TestCaseConflict();
            Conflict.Message = message;
            Conflict.TestCase = testCase;
            Conflict.TfsTestCase = tfsTestCase;
        }
    }
}
