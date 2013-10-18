using TfsApi.Queries.Contracts;
using TfsApi.WorkItemTracking;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TfsApi.Queries
{
    internal class QueryRunner : IQueryRunner
    {
        private readonly Uri requestUri;
        private readonly TfsApi.Contracts.ITfsCredentials tfsCredentials;
        readonly string _projectName;

        public QueryRunner(Uri requestUri, string projectName, TfsApi.Contracts.ITfsCredentials tfsCredentials)
        {
            // TODO: Complete member initialization
            this.requestUri = requestUri;
            _projectName = projectName;
            this.tfsCredentials = tfsCredentials;
        }

        public WorkItemCollection Execute(string query)
        {
            WorkItemStore store = WorkItemStoreFactory.GetWorkItemStore(requestUri, tfsCredentials);
            query = query.Replace("@project", "'" + _projectName + "'");
            return store.Query(query);
        }

        public WorkItemCollection ExecuteSavedQuery(string queryFullname)
        {
            var workItemStore = WorkItemStoreFactory.GetWorkItemStore(requestUri, tfsCredentials);
            var queryHirerarchy = workItemStore.Projects[_projectName].QueryHierarchy;
            string query = GetQueryText(queryHirerarchy, queryFullname);
            if (string.IsNullOrEmpty(query))
            {
                throw new Exception("Couldn't find a wiql query for '" + queryFullname + "'.");
            }
            return Execute(query);
        }

        private string GetQueryText(QueryFolder queryFolder, string queryFullname)
        {
            string result = null;
            string thisLevelName = queryFullname.TrimStart('/');
            string queryPathLeft = string.Empty;
            if (thisLevelName.Contains("/"))
            {
                thisLevelName = thisLevelName.Remove(thisLevelName.IndexOf("/"));
                queryPathLeft = queryFullname.TrimStart('/');
                queryPathLeft = queryPathLeft.Remove(0, thisLevelName.Length);
            }
            if (queryFolder.Name == thisLevelName)
            {
                foreach (QueryItem query in queryFolder)
                {
                    if (query is QueryFolder)
                    {
                        result = GetQueryText(query as QueryFolder, queryPathLeft);
                    }
                    else if (query is QueryDefinition)
                    {
                        if (query.Name == queryPathLeft.TrimStart('/'))
                        {
                            result = (query as QueryDefinition).QueryText;
                        }
                    }
                    if (result != null)
                    {
                        break;
                    }
                }
            }
            return result;
        }
    }
}
