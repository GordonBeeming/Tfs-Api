// // <copyright file="ITfsCredentials.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the ITfsCredentials.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Contracts
{
    #region

    using Microsoft.TeamFoundation.Client;
    using System.Net;

    #endregion

    public interface ITfsCredentials
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the credentials.
        /// </summary>
        /// <returns></returns>
        TfsClientCredentials GetCredentials();

        #endregion
    }
}