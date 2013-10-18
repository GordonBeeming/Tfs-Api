using TfsApi.WorkItemTracking.Delegates;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsApi.WorkItemTracking
{
    public static class WorkItemStoreFactory
    {
        public static WorkItemStore GetWorkItemStore(Uri requestUri, TfsApi.Contracts.ITfsCredentials tfsCredentials = null, bool bypassRules = false)
        {
            TfsTeamProjectCollection tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(requestUri);
            return new WorkItemStore(tfs, bypassRules ? WorkItemStoreFlags.BypassRules : WorkItemStoreFlags.None);
        }

        internal static void Reset()
        {
            // nothing
        }
    }
}
