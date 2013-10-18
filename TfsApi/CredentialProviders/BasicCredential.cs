// // <copyright file="BasicCredential.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the BasicCredential.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.CredentialProviders
{
    #region

    using Microsoft.TeamFoundation.Client;
    using System.Net;
    using System.Security;
    using TfsApi.Contracts;

    #endregion

    public sealed class BasicCredential : ITfsCredentials
    {
        #region Fields

        private readonly NetworkCredential _networkCredential;

        #endregion

        #region Constructors and Destructors

        public BasicCredential(string userName, string password)
        {
            this._networkCredential = new NetworkCredential(userName, password);
        }

        public BasicCredential(string userName, SecureString password)
        {
            this._networkCredential = new NetworkCredential(userName, password);
        }

        public BasicCredential(string userName, SecureString password, string domain)
        {
            this._networkCredential = new NetworkCredential(userName, password, domain);
        }

        public BasicCredential(string userName, string password, string domain)
        {
            this._networkCredential = new NetworkCredential(userName, password, domain);
        }

        #endregion

        #region Public Methods and Operators

        public TfsClientCredentials GetCredentials()
        {
            return new TfsClientCredentials(new WindowsCredential(this._networkCredential));
        }

        #endregion
    }
}