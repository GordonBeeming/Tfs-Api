using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsApi.WorkItemTracking.Delegates
{
    public delegate List<WorkItem> WorkItemStoreWork(WorkItemStore store);
}
