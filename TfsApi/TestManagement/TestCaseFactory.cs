using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsApi.TestManagement.Contracts;
using TfsApi.TestManagement.Workers;

namespace TfsApi.TestManagement
{
   public static class TestCaseFactory
    {
       public static ITestCase CreateDefaultTestCase()
       {
           return new TestCase();
       }

       internal static void Reset()
       {
           // nothing
       }
    }
}
