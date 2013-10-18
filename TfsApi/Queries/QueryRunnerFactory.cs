using TfsApi.Queries.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsApi.Queries
{
    public static class QueryRunnerFactory
    {
        public static IQueryRunner CreateInstance(Uri requestUri, string projectName, TfsApi.Contracts.ITfsCredentials tfsCredentials = null)
        {
            return new QueryRunner(requestUri, projectName, tfsCredentials);
        }

        internal static void Reset()
        {
            // nothing
        }
    }
}
