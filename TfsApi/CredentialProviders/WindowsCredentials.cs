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

    public sealed class WindowsCredentials : ITfsCredentials
    {
        #region Fields

        bool _allowInteractive = false;

        #endregion

        #region Constructors and Destructors

        public WindowsCredentials(bool allowInteractive = false)
        {

        }

        #endregion

        #region Public Methods and Operators

        public TfsClientCredentials GetCredentials()
        {
            return new TfsClientCredentials(new WindowsCredential(), _allowInteractive);
        }

        #endregion
    }
}