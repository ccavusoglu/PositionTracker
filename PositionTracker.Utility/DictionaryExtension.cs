using System;
using System.Collections.Generic;
using System.Linq;

namespace PositionTracker.Utility
{
    public static class DictionaryExtension
    {
        public static void AddOrSum<T>(this IDictionary<T, decimal> dict, T key, decimal value)
        {
            if (dict.ContainsKey(key))
                dict[key] += value;
            else
                dict.Add(key, value);
        }

        public static void AddOrAppend<T, TV>(this IDictionary<T, IList<TV>> dict, T key, TV value,
            Action<T, TV> updateValueFactory)
        {
            if (dict.ContainsKey(key))
                updateValueFactory.Invoke(key, value);
            else
                dict.Add(key, new List<TV> {value});
        }

        public static TV Get<T, TV>(this IDictionary<T, TV> dict, T key)
        {
            return dict.ContainsKey(key) ? dict[key] : default(TV);
        }

        /// <summary>
        /// Add if not exist.
        /// </summary>
        public static TV GetOrAddEmpty<T, TV>(this IDictionary<T, TV> dict, T key) where TV : new()
        {
            if (!dict.ContainsKey(key)) dict.Add(key, new TV());

            return dict[key];
        }
    }
}