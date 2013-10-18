// // <copyright file="IProcessTemplates.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the IProcessTemplates.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Administration.Contracts
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;

    using TfsApi.Administration.Dto;

    #endregion

    public interface IProcessTemplates : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Deletes the template.
        /// </summary>
        /// <param name="name">The name.</param>
        void DeleteTemplate(string name);

        /// <summary>
        ///     Downloads the template and returns a byte[].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        byte[] DownloadTemplateAndReturnByteArray(string name);

        /// <summary>
        ///     Downloads the template and returns a path to the zip file.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        string DownloadTemplateAndReturnPathToZip(string name);

        /// <summary>
        ///     Lists the process templates.
        /// </summary>
        /// <returns></returns>
        List<ProcessTemplate> ListProcessTemplates();

        /// <summary>
        ///     Sets the default template.
        /// </summary>
        /// <param name="name">The name.</param>
        void MakeDefaultTemplate(string name);

        /// <summary>
        ///     Uploads the template.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="zipFileName">Name of the zip file.</param>
        /// <param name="metaData">The meta data.</param>
        void UploadTemplate(string name, string description, string zipFileName, string metaData = null);

        /// <summary>
        ///     Uploads the template.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="zipFileStream">The zip file stream.</param>
        /// <param name="metaData">The meta data.</param>
        void UploadTemplate(string name, string description, Stream zipFileStream, string metaData = null);

        /// <summary>
        ///     Uploads the template.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="zipFileByteArray">The zip file byte array.</param>
        /// <param name="metaData">The meta data.</param>
        void UploadTemplate(string name, string description, byte[] zipFileByteArray, string metaData = null);

        #endregion
    }
}