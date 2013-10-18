// // <copyright file="ProcessTemplateFactory.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the ProcessTemplateFactory.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Administration
{
    #region

    using System;
    using System.Collections.Concurrent;

    using TfsApi.Administration.Contracts;
    using TfsApi.Contracts;

    #endregion

    public static class ProcessTemplateFactory
    {
        #region Static Fields

        private static readonly ConcurrentDictionary<Uri, IProcessTemplates> ProcessTemplateManagers = new ConcurrentDictionary<Uri, IProcessTemplates>();

        #endregion

        #region Public Methods and Operators

        public static IProcessTemplates CreateProcessTemplateMananger(Uri tfsCollectionUri, ITfsCredentials tfsCredentials = null)
        {
            IProcessTemplates result;
            if (ProcessTemplateManagers.ContainsKey(tfsCollectionUri))
            {
                result = ProcessTemplateManagers[tfsCollectionUri];
            }
            else
            {
                result = new ProcessTemplates(tfsCollectionUri, tfsCredentials);
                ProcessTemplateManagers.AddOrUpdate(tfsCollectionUri, result, (key, oldValue) => result);
            }

            return result;
        }

        #endregion

        internal static void Reset()
        {
            ProcessTemplateManagers.Clear();
        }
    }
}