#region

using System.Collections.Generic;

using Microsoft.TeamFoundation.Server;

#endregion

public static class PropertyInfoExtensions
{
    #region Public Methods and Operators

    public static Dictionary<string, string> ConvertToDictionary(this Property[] property)
    {
        var result = new Dictionary<string, string>();

        foreach (Property item in property)
        {
            result.Add(item.Name, item.Value);
        }

        return result;
    }

    #endregion
}