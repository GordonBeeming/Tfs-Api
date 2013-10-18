using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsApi.TestManagement.Contracts;
using TfsApi.TestManagement.Workers;

namespace TfsApi.TestManagement
{
    public static class TestSuiteFactory
    {
        public static ITestSuite CreateDefaultTestSuite()
        {
            return new TestSuite();
        }

        internal static void Reset()
        {
            // nothing
        }
    }
}
