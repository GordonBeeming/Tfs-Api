using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsApi.Administration;
using TfsApi.Queries;
using TfsApi.TestManagement;
using TfsApi.WorkItemTracking;
using TfsTeamProjectCollectionFactory = TfsApi.Administration.TfsTeamProjectCollectionFactory;
namespace TfsApi
{
    public static class Api
    {
        public static void Refresh()
        {
            //Administration
            AreaManagerFactory.Reset();
            GlobalListFactory.Reset();
            IterationManagerFactory.Reset();
            ProcessTemplateFactory.Reset();
            TeamManagerFactory.Reset();
            TeamProjectFactory.Reset();
            TfsTeamProjectCollectionFactory.Reset();
            TeamProjectCollectionFactory.Reset();

            //Queries
            QueryRunnerFactory.Reset();

            //TestManagement
            TestCaseFactory.Reset();
            TestCaseStepFactory.Reset();
            TestSuiteFactory.Reset();
            TestSuiteManagerFactory.Reset();

            //WorkItemTracking
            WorkItemStoreFactory.Reset();
        }
 
    }
}
