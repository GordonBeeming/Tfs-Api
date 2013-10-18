using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsApi.Administration.Workers;
using TfsApi.Administration.Contracts;

namespace TfsApi.Administration
{
    public class GlobalListFactory
    {
        public static IGlobalList GetGlobalList(Uri requestUri, string globalListName, TfsApi.Contracts.ITfsCredentials tfsCredentials = null)
        {
            return new GlobalList(requestUri, globalListName, tfsCredentials);
        }

        internal static void Reset()
        {
            // nothing
        }
    }
}
