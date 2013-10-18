// // <copyright file="Defaults.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the Defaults.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi
{
    #region

    using System;
    using System.Collections.Concurrent;

    using TfsApi.Contracts;

    #endregion

    public sealed class Defaults
    {
        #region Static Fields

        private static readonly object SyncRoot = new object();

        private static readonly ConcurrentDictionary<Uri, ITfsCredentials> _tfsCredentials = new ConcurrentDictionary<Uri, ITfsCredentials>();

        private static volatile Defaults instance;

        #endregion

        #region Constructors and Destructors

        private Defaults()
        {
        }

        #endregion

        #region Public Properties

        public static Defaults Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Defaults();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static ITfsCredentials GetDefaultCredentials(Uri tfsUri)
        {
            ITfsCredentials result = null;
            if (_tfsCredentials.ContainsKey(tfsUri))
            {
                result = _tfsCredentials[tfsUri];
            }

            return result;
        }

        public static void SetDefaultCredentials(Uri tfsUri, ITfsCredentials tfsCredentials)
        {
            _tfsCredentials.AddOrUpdate(tfsUri, tfsCredentials, (key, oldValue) => tfsCredentials);
        }

        public static void GetDefaultVisualStudioEnvironment(Uri tfsUri, ITfsCredentials tfsCredentials)
        {
            _tfsCredentials.AddOrUpdate(tfsUri, tfsCredentials, (key, oldValue) => tfsCredentials);
        }

        #endregion
    }
}