using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static TValue Get<TValue>(this IDictionary<string, object> dictionary, string key)
    {
        return (TValue)dictionary[key];
    }
}