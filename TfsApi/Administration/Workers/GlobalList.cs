using TfsApi.Administration.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace TfsApi.Administration.Workers
{
    internal class GlobalList : IGlobalList
    {
        private Uri requestUri;
        private string globalListName;
        private TfsApi.Contracts.ITfsCredentials tfsCredentials;

        private readonly List<string> _items;

        public List<string> Items
        {
            get { return _items; }
        }

        public bool ExistsOnServer { get; private set; }

        public GlobalList(Uri requestUri, string globalListName, TfsApi.Contracts.ITfsCredentials tfsCredentials)
        {
            _items = new List<string>();
            this.requestUri = requestUri;
            this.globalListName = globalListName;
            this.tfsCredentials = tfsCredentials;

            LoadListFromServer(globalListName);
        }

        public void LoadListFromServer(string globalListName)
        {
            ExistsOnServer = false;

            XDocument doc = LoadGlobalListFromServerAsXmlDocument();

            XElement globalListElement = FindGlobalListElementIn(doc);
            ExistsOnServer = globalListElement != null;

            if (globalListElement != null)
            {
                foreach (XElement itemNode in globalListElement.DescendantNodes())
                {
                    Items.Add((string)itemNode.Attribute("value"));
                }
            }

        }

        private XDocument LoadGlobalListFromServerAsXmlDocument()
        {
            XDocument doc = null;
            Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore store = WorkItemTracking.WorkItemStoreFactory.GetWorkItemStore(requestUri,tfsCredentials);
            doc = XDocument.Parse(store.ExportGlobalLists().OuterXml);

            if (doc == null)
            {
                throw new ArgumentNullException("doc", "Unable to load any global list data from the server '" + requestUri + "'.");
            }
            return doc;
        }

        public void ClearList()
        {
            _items.Clear();
        }

        public void AddToList(string value)
        {
            _items.Add(value);
        }

        public void AddToListDistinct(string value)
        {
            if (!ValueExistsInList(value))
            {
                AddToList(value);
            }
        }

        public bool ValueExistsInList(string value)
        {
            foreach (var item in _items)
            {
                if (string.Compare(item, value, true) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveFromList(string value)
        {
            if (_items.IndexOf(value) > -1)
            {
                _items.Remove(value);
            }
        }

        public void SaveChanges(bool createIfNotExists)
        {
            XDocument docFromServer = LoadGlobalListFromServerAsXmlDocument();
            XDocument doc = XDocument.Parse(docFromServer.ToString());
            var globalListElement = FindGlobalListElementIn(doc);

            globalListElement = CreateSingleGlobalListElement(createIfNotExists, doc, globalListElement);

            PopulateGlobalListElementWithItems(globalListElement);

            var newXmlDocument = new XmlDocument();
            newXmlDocument.InnerXml = doc.ToString();


            Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore store = WorkItemTracking.WorkItemStoreFactory.GetWorkItemStore(requestUri,tfsCredentials);
            store.ImportGlobalLists(newXmlDocument.DocumentElement);
        }

        private void PopulateGlobalListElementWithItems(XElement globalListElement)
        {
            foreach (string listItem in Items)
            {
                globalListElement.Add(new XElement("LISTITEM", new XAttribute("value", listItem)));
            }
        }

        private XElement CreateSingleGlobalListElement(bool createIfNotExists, XDocument doc, XElement globalListElement)
        {
            if (globalListElement == null)
            {
                if (!createIfNotExists)
                {
                    throw new Exception("The Global List '" + globalListName + "' doesn't exists on the server '" + requestUri + "'. Please create the create the global list on the server or specify the 'createIfNotExists' property as true.");
                }
                globalListElement = new XElement("GLOBALLIST", new XAttribute("name", globalListName));
                doc.Root.Add(globalListElement);
            }
            else
            {
                globalListElement.RemoveNodes();
            }
            return globalListElement;
        }

        private XElement FindGlobalListElementIn(XDocument doc)
        {
            var globalListElement = (from el in doc.Root.Descendants() where (string)el.Attribute("name") == globalListName select el).FirstOrDefault();
            return globalListElement;
        }
    }
}
