namespace TfsApi.TestManagement
{
    #region

    using System;
    using System.Collections.Concurrent;

    using TfsApi.Contracts;
    using TfsApi.TestManagement.Contracts;
    using TfsApi.TestManagement.Workers;

    #endregion

    public static class TestSuiteManagerFactory
    {
        #region Static Fields

        private static readonly ConcurrentDictionary<Uri, ITestSuiteManager> TestSuiteManagers = new ConcurrentDictionary<Uri, ITestSuiteManager>();

        #endregion

        #region Public Methods and Operators

        public static ITestSuiteManager CreateTestSuiteMananger(Uri tfsCollectionUri, ITfsCredentials tfsCredentials = null)
        {
            ITestSuiteManager result;
            if (TestSuiteManagers.ContainsKey(tfsCollectionUri))
            {
                result = TestSuiteManagers[tfsCollectionUri];
            }
            else
            {
                result = new TestSuiteManager(tfsCollectionUri, tfsCredentials);
                TestSuiteManagers.AddOrUpdate(tfsCollectionUri, result, (key, oldValue) => result);
            }

            return result;
        }

        #endregion

        internal static void Reset()
        {
            TestSuiteManagers.Clear();
        }
    }
}