using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TfsApi.Queries.Contracts
{
    public interface IQueryRunner
    {
        WorkItemCollection Execute(string query);

        WorkItemCollection ExecuteSavedQuery(string queryFullname);
    }
}
