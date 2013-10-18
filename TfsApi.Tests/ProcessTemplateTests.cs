namespace TfsApi.Tests
{
    #region

    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TfsApi.Administration;
    using TfsApi.Administration.Contracts;
    using TfsApi.Administration.Dto;
    using TfsApi.Administration.Tests;

    #endregion

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ProcessTemplateTests
    {
        #region Public Methods and Operators

        [TestMethod]
        public void FullDownloadDeleteAndUpload_Default_FileExistsThenTemplateNotInTfsAndThenNewTemplateInTfs()
        {
            IProcessTemplates processTemplates = ProcessTemplateFactory.CreateProcessTemplateMananger(TestConstants.TfsCollectionUri);
            int currentDefaultTemplateID = processTemplates.ListProcessTemplates().OrderBy(o => o.Rank).FirstOrDefault().TemplateId;
            ProcessTemplate processTemplateToAction = processTemplates.ListProcessTemplates().OrderByDescending(o => o.Rank).FirstOrDefault(); // not the default
            Assert.IsNotNull(processTemplateToAction);
            Assert.AreNotEqual(processTemplateToAction.TemplateId, currentDefaultTemplateID, "The selected template is the current default process template, please in sure there is 2 or more templates in the current Tfs Collection.");

            int templateCountToStartWith = processTemplates.ListProcessTemplates().Count;

            // download template
            string downloadedTemplateZipFile = processTemplates.DownloadTemplateAndReturnPathToZip(processTemplateToAction.Name);
            Assert.IsTrue(File.Exists(downloadedTemplateZipFile), "The template didn't download");

            // delete the process template
            processTemplates.DeleteTemplate(processTemplateToAction.Name);
            ProcessTemplate processTemplateAfterDelete = processTemplates.ListProcessTemplates().FirstOrDefault(o => string.Compare(o.Name, processTemplateToAction.Name, true) == 0);
            Assert.IsNull(processTemplateAfterDelete, "The process template didn't delete properly (processTemplateAfterDelete != null).");
            int templateCountAfterDelete = processTemplates.ListProcessTemplates().Count;
            Assert.IsTrue(templateCountToStartWith > templateCountAfterDelete, "The process template didn't delete properly (templateCountToStartWith > templateCountAfterDelete).");

            // upload process template
            processTemplates.UploadTemplate(processTemplateToAction.Name, processTemplateToAction.Description, downloadedTemplateZipFile);
            ProcessTemplate processTemplateAfterUpload = processTemplates.ListProcessTemplates().FirstOrDefault(o => string.Compare(o.Name, processTemplateToAction.Name, true) == 0);
            Assert.IsNotNull(processTemplateAfterUpload, "The process template didn't upload properly (processTemplateAfterUpload == null).");
            int templateCountAfterUpload = processTemplates.ListProcessTemplates().Count;
            Assert.IsTrue(templateCountAfterUpload > templateCountAfterDelete, "The process template didn't upload properly (templateCountAfterUpload > templateCountAfterDelete).");
            Assert.IsTrue(templateCountToStartWith == templateCountAfterUpload, "The process template didn't upload properly (templateCountToStartWith == templateCountAfterUpload).");

            // check first template id is not current template id
            Assert.IsTrue(processTemplateToAction.TemplateId != processTemplateAfterUpload.TemplateId, "Something didn't work properly.");

            // check default template id hasn't changed
            int currentDefaultTemplateAfterWorking = processTemplates.ListProcessTemplates().OrderBy(o => o.Rank).FirstOrDefault().TemplateId;
            Assert.IsTrue(currentDefaultTemplateID == currentDefaultTemplateAfterWorking, "The Default process template ID has changed.");

            // Make default template
            processTemplates.MakeDefaultTemplate(processTemplateAfterUpload.Name);
            int currentDefaultTemplateAfterMakingDefault = processTemplates.ListProcessTemplates().OrderBy(o => o.Rank).FirstOrDefault().TemplateId;
            Assert.IsTrue(processTemplateAfterUpload.TemplateId == currentDefaultTemplateAfterMakingDefault, "The Default process template wasn't set correctly.");
        }

        [TestMethod]
        public void InstanceFromFactory()
        {
            IProcessTemplates processTemplates = ProcessTemplateFactory.CreateProcessTemplateMananger(TestConstants.TfsCollectionUri);

            Assert.IsInstanceOfType(processTemplates, typeof(IProcessTemplates));
        }

        [TestMethod]
        public void ListProcessTemplates_Defaults_ListGreaterThan0()
        {
            // arrange
            IProcessTemplates processTemplates = ProcessTemplateFactory.CreateProcessTemplateMananger(TestConstants.TfsCollectionUri);

            // act
            List<ProcessTemplate> actual = processTemplates.ListProcessTemplates();

            // assert
            Assert.IsNotNull(actual);

            Assert.IsTrue(actual.Count > 0);
        }

        #endregion
    }
}