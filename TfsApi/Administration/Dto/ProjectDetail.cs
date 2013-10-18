namespace TfsApi.Administration.Dto
{
    #region

    using System;

    #endregion

    public class ProjectDetail
    {
        public Uri CollectionUri { get; set; }

        public string ProjectName { get; set; }

        public Uri ProjectUri
        {
            get
            {
                return new Uri(CollectionUri + "/" + ProjectName);
            }
        }
    }
}