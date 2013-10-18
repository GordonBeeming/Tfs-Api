using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TfsApi.Administration.Contracts
{
    public interface IGlobalList
    {
        void ClearList();

        List<string> Items { get; }

        void LoadListFromServer(string globalListName);

        void AddToList(string value);

        void AddToListDistinct(string value);

        void SaveChanges(bool createIfNotExists);

        void RemoveFromList(string value);

        bool ValueExistsInList(string value);

        bool ExistsOnServer { get; }
    }
}
