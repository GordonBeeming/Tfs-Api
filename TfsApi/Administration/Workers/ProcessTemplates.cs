// // <copyright file="ProcessTemplates.cs" company="Binary Digit">
// //   This source code is copyright Binary Digit. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the ProcessTemplates.cs type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace TfsApi.Administration
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Xml;

    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.Framework.Common;
    using Microsoft.TeamFoundation.Server;

    using TfsApi.Administration.Dto;
    using TfsApi.Contracts;
    using TfsApi.Utilities;

    using IProcessTemplate = TfsApi.Administration.Contracts.IProcessTemplates;

    #endregion

    internal class ProcessTemplates : IProcessTemplate
    {
        #region Fields

        private readonly Uri _collectionUri;

        private readonly TeamProjectCollection _teamProjectCollection;

        private readonly ITeamProjectCollectionService _teamProjectCollectionService;

        private readonly TfsTeamProjectCollection _tfsTeamProjectCollection;

        /// <summary>
        ///     Internal variable which checks if Dispose has already been called
        /// </summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        public ProcessTemplates(Uri collectionUri, ITfsCredentials tfsCredentials = null)
        {
            this._collectionUri = collectionUri;

            this._tfsTeamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(this._collectionUri, tfsCredentials);

            this._teamProjectCollectionService = this._tfsTeamProjectCollection.GetConfigurationServer().GetService<ITeamProjectCollectionService>();
            this._teamProjectCollection = this._teamProjectCollectionService.GetCollection(this._tfsTeamProjectCollection.InstanceId);
        }

        #endregion

        #region Public Methods and Operators

        public void DeleteTemplate(string name)
        {
            if (this._teamProjectCollection.State == TeamFoundationServiceHostStatus.Started)
            {
                var processTemplates = this._tfsTeamProjectCollection.GetService<IProcessTemplates>();

                ProcessTemplate pt = (from o in this.ListProcessTemplates()
                                      where string.Compare(o.Name, name, true) == 0
                                      select o).FirstOrDefault();
                if (pt == null)
                {
                    throw new Exception("The process template name '" + name + "' doesn't exist in the collection '" + this._collectionUri + "'.");
                }
                else
                {
                    processTemplates.DeleteTemplate(pt.TemplateId);
                }
            }
            else
            {
                throw new Exception("The current state of the team project collection is " + this._teamProjectCollection.State.ToString() + " and is not usable");
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Call the private Dispose(bool) helper and indicate 
            // that we are explicitly disposing
            this.Dispose(true);

            // Tell the garbage collector that the object doesn't require any
            // cleanup when collected since Dispose was called explicitly.
            GC.SuppressFinalize(this);
        }

        public byte[] DownloadTemplateAndReturnByteArray(string name)
        {
            byte[] result = null;
            if (this._teamProjectCollection.State == TeamFoundationServiceHostStatus.Started)
            {
                var processTemplates = this._tfsTeamProjectCollection.GetService<IProcessTemplates>();

                ProcessTemplate pt = (from o in this.ListProcessTemplates()
                                      where string.Compare(o.Name, name, true) == 0
                                      select o).FirstOrDefault();
                if (pt == null)
                {
                    throw new Exception("The process template name '" + name + "' doesn't exist in the collection '" + this._collectionUri + "'.");
                }
                else
                {
                    string tempPath = processTemplates.GetTemplateData(pt.TemplateId);
                    result = StreamUtility.ReadByteArrayFromFilename(tempPath);
                }
            }
            else
            {
                throw new Exception("The current state of the team project collection is " + this._teamProjectCollection.State.ToString() + " and is not usable");
            }

            return result;
        }

        public string DownloadTemplateAndReturnPathToZip(string name)
        {
            if (this._teamProjectCollection.State == TeamFoundationServiceHostStatus.Started)
            {
                var processTemplates = this._tfsTeamProjectCollection.GetService<IProcessTemplates>();

                ProcessTemplate pt = (from o in this.ListProcessTemplates()
                                      where string.Compare(o.Name, name, true) == 0
                                      select o).FirstOrDefault();
                if (pt == null)
                {
                    throw new Exception("The process template name '" + name + "' doesn't exist in the collection '" + this._collectionUri + "'.");
                }
                else
                {
                    return processTemplates.GetTemplateData(pt.TemplateId);
                }
            }
            else
            {
                throw new Exception("The current state of the team project collection is " + this._teamProjectCollection.State.ToString() + " and is not usable");
            }
        }

        public List<ProcessTemplate> ListProcessTemplates()
        {
            if (this._teamProjectCollection.State == TeamFoundationServiceHostStatus.Started)
            {
                var processTemplates = this._tfsTeamProjectCollection.GetService<IProcessTemplates>();

                var result = new List<ProcessTemplate>();
                foreach (TemplateHeader item in processTemplates.TemplateHeaders())
                {
                    result.Add(item);
                }

                return result;
            }
            else
            {
                throw new Exception("The current state of the team project collection is " + this._teamProjectCollection.State.ToString() + " and is not usable");
            }

            return null;
        }

        public void MakeDefaultTemplate(string name)
        {
            if (this._teamProjectCollection.State == TeamFoundationServiceHostStatus.Started)
            {
                var processTemplates = this._tfsTeamProjectCollection.GetService<IProcessTemplates>();

                ProcessTemplate pt = (from o in this.ListProcessTemplates()
                                      where string.Compare(o.Name, name, true) == 0
                                      select o).FirstOrDefault();
                if (pt == null)
                {
                    throw new Exception("The process template name '" + name + "' doesn't exist in the collection '" + this._collectionUri + "'.");
                }
                else
                {
                    processTemplates.MakeDefaultTemplate(pt.TemplateId);
                }
            }
            else
            {
                throw new Exception("The current state of the team project collection is " + this._teamProjectCollection.State.ToString() + " and is not usable");
            }
        }

        public void UploadTemplate(string name, string description, string zipFileName, string metaData = null)
        {
            if (this._teamProjectCollection.State == TeamFoundationServiceHostStatus.Started)
            {
                var processTemplates = this._tfsTeamProjectCollection.GetService<IProcessTemplates>();

                if (string.IsNullOrEmpty(metaData))
                {
                    metaData = this.ReadMetaData(zipFileName);
                }

                processTemplates.AddUpdateTemplate(name, description, metaData, "visible", zipFileName);
            }
            else
            {
                throw new Exception("The current state of the team project collection is " + this._teamProjectCollection.State.ToString() + " and is not usable");
            }
        }

        public void UploadTemplate(string name, string description, Stream zipFileStream, string metaData = null)
        {
            if (this._teamProjectCollection.State == TeamFoundationServiceHostStatus.Started)
            {
                this.UploadTemplate(name, description, StreamUtility.ConvertStreamToByteArray(zipFileStream));
            }
            else
            {
                throw new Exception("The current state of the team project collection is " + this._teamProjectCollection.State.ToString() + " and is not usable");
            }
        }

        public void UploadTemplate(string name, string description, byte[] zipFileByteArray, string metaData = null)
        {
            if (this._teamProjectCollection.State == TeamFoundationServiceHostStatus.Started)
            {
                string filename = Path.GetTempFileName();
                StreamUtility.WriteToFileSystem(filename, zipFileByteArray);
                this.UploadTemplate(name, description, filename);
            }
            else
            {
                throw new Exception("The current state of the team project collection is " + this._teamProjectCollection.State.ToString() + " and is not usable");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: Managed cleanup code here, while managed refs still valid
            }

            // TODO: Unmanaged cleanup code here
            this.disposed = true;
        }

        private string ReadMetaData(string zipFilename)
        {
            string result = null;

            FileStream fs = null;
            try
            {
                string tempFolderPath = Path.GetTempPath() + "\\" + Guid.NewGuid().ToString("N") + "\\";
                fs = new FileStream(zipFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var za = new ZipArchive(fs, ZipArchiveMode.Read);
                za.ExtractToDirectory(tempFolderPath);

                foreach (FileInfo processTemplateXml in new DirectoryInfo(tempFolderPath).GetFiles("ProcessTemplate.xml", SearchOption.AllDirectories))
                {
                    var doc = new XmlDocument();
                    doc.Load(processTemplateXml.FullName);
                    XmlNode xnMetaData = doc.SelectSingleNode("//metadata");
                    result = xnMetaData.OuterXml;
                    break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No meta data was supplied and there was an error trying to extract meta data. Please see inner exception for more info.", ex);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

            return result;
        }

        #endregion
    }
}