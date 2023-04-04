using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EST.MIT.InvoiceImporter.Function.Services;

public static class InvoiceUtil
{
    public static uint GetRowIndex(string cellName)
    {
        var regex = new Regex(@"\d+");
        var match = regex.Match(cellName);
        return uint.Parse(match.Value);
    }

    public static string GetColumnName(string cellName)
    {
        var regex = new Regex("[A-Za-z]+");
        var match = regex.Match(cellName);
        return match.Value;
    }

    public static int CompareColumn(string column1, string column2)
    {
        if (column1.Length > column2.Length)
            return 1;

        if (column1.Length < column2.Length)
            return -1;

        return string.Compare(column1, column2, true);
    }

    public static T DictionaryToObject<T>(Dictionary<int, string> dict) where T : new()
    {
        var obj = new T();
        var properties = typeof(T).GetProperties();

        for (int i = 0; i < properties.Length; i++)
        {
            if (dict.TryGetValue(i, out string value))
            {
                var propertyType = properties[i].PropertyType;
                var convertedValue = Convert.ChangeType(value, propertyType);
                properties[i].SetValue(obj, convertedValue);
            }
        }

        return obj;
    }
}
