namespace TfsApi.Administration.Helpers
{
    #region

    using System;

    using Microsoft.TeamFoundation.Server;

    using TfsApi.Administration.Enums;

    #endregion

    internal static class TfsNodeStructureHelper
    {
        #region Public Methods and Operators

        public static NodeInfo AddNode(ICommonStructureService commonStructureService, string elementPath, string projectName, eStructureType nodeType)
        {
            NodeInfo retVal;
            string rootNodePath = "\\" + projectName + "\\" + nodeType.ToString();

            if (CheckIfPathAlreadyExists(commonStructureService, elementPath, rootNodePath))
            {
                return null;
            }

            int backSlashIndex = GetBackSlashIndex(elementPath);
            string newpathname = GetNewPathName(elementPath, backSlashIndex);
            string path = GetActualNodePath(elementPath, backSlashIndex);
            string pathRoot = rootNodePath + path;
            NodeInfo previousPath = GetPreviousPath(commonStructureService, pathRoot);

            if (previousPath == null)
            {
                // call this method to create the parent paths.
                previousPath = AddNode(commonStructureService, path, projectName, nodeType);
            }

            string newPathUri = commonStructureService.CreateNode(newpathname, previousPath.Uri);
            return commonStructureService.GetNode(newPathUri);
        }

        #endregion

        #region Methods

        private static bool CheckIfPathAlreadyExists(ICommonStructureService commonStructureService, string elementPath, string rootNodePath)
        {
            bool result = false;
            if (!elementPath.StartsWith("\\"))
            {
                elementPath = "\\" + elementPath;
            }

            string newPath = rootNodePath + elementPath;
            try
            {
                if (DoesNodeExistInPath(commonStructureService, newPath))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                if (!ExceptionMeansThatPathDoesNotExists(ex))
                {
                    throw;
                }
            }

            return result;
        }

        private static bool DoesNodeExistInPath(ICommonStructureService commonStructureService, string newPath)
        {
            bool result = false;
            NodeInfo retVal = commonStructureService.GetNodeFromPath(newPath);
            if (retVal != null)
            {
                result = true;
            }

            return result;
        }

        private static bool ExceptionMeansThatPathDoesNotExists(Exception ex)
        {
            return ex.Message.Contains("Invalid path.") || ex.Message.Contains("The following node does not exist");
        }

        private static string GetActualNodePath(string elementPath, int backSlashIndex)
        {
            return backSlashIndex == 0 ? string.Empty : elementPath.Substring(0, backSlashIndex);
        }

        private static int GetBackSlashIndex(string elementPath)
        {
            int backSlashIndex = elementPath.LastIndexOf("\\");
            return backSlashIndex;
        }

        private static string GetNewPathName(string elementPath, int backSlashIndex)
        {
            string newpathname = elementPath.Substring(backSlashIndex + 1);
            return newpathname;
        }

        private static NodeInfo GetPreviousPath(ICommonStructureService commonStructureService, string pathRoot)
        {
            NodeInfo result = null;
            try
            {
                result = commonStructureService.GetNodeFromPath(pathRoot);
            }
            catch (Exception ex)
            {
                if (!ExceptionMeansThatPathDoesNotExists(ex))
                {
                    throw;
                }
            }

            return result;
        }

        #endregion
    }
}