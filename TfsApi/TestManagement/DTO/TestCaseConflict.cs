using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TfsApi.TestManagement.DTO
{
    public struct TestCaseConflict
    {
        public string Message { get; set; }
        public Microsoft.TeamFoundation.TestManagement.Client.ITestCase TfsTestCase { get; set; }
        public TfsApi.TestManagement.Contracts.ITestCase TestCase { get; set; }
    }
}
