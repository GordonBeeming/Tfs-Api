// // <copyright file="ProcessTemplate.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the ProcessTemplate.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Administration.Dto
{
    #region

    using Microsoft.TeamFoundation.Server;

    #endregion

    public sealed class ProcessTemplate
    {
        #region Public Properties

        public string Description { get; set; }

        public string Metadata { get; set; }

        public string Name { get; set; }

        public int Rank { get; set; }

        public string State { get; set; }

        public int TemplateId { get; set; }

        #endregion

        #region Public Methods and Operators

        public static implicit operator ProcessTemplate(TemplateHeader th)
        {
            return new ProcessTemplate {
                                           Description = th.Description, 
                                           Metadata = th.Metadata, 
                                           Name = th.Name, 
                                           Rank = th.Rank, 
                                           State = th.State, 
                                           TemplateId = th.TemplateId
                                       };
        }

        #endregion
    }
}