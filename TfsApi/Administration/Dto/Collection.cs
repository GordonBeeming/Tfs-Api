namespace TfsApi.Administration.Dto
{
    #region

    using System;

    #endregion

    public sealed class Collection
    {
        #region Public Properties

        public string CollectionDescription { get; set; }

        public string CollectionName { get; internal set; }

        public bool IsDefault { get; internal set; }

        public Guid TeamProjectCollectionID { get; set; }

        public Uri Url { get; internal set; }

        #endregion
    }
}