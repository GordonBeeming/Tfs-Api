using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TfsTeamProjectCollectionExtensions
{
    public static TfsConfigurationServer GetConfigurationServer(this TfsTeamProjectCollection tfsTeamProjectCollection)
    {
        try
        {
            return tfsTeamProjectCollection.ConfigurationServer;
        }
        catch (TeamFoundationServiceUnavailableException ex)
        {
            if (tfsTeamProjectCollection.Name == tfsTeamProjectCollection.Uri.ToString() && tfsTeamProjectCollection.SessionId != Guid.Empty && ex.Message.Contains(@"The remote server returned an error: (404) Not Found."))
            {
                throw new Exception("It is possible that no default collection is present on the server. Try setting the default collection or specifying the default collection in your Url. See inner exceptions for more details" + Environment.NewLine + Environment.NewLine, ex);
            }
            else
            {
                throw;
            }
        }
    }
}
