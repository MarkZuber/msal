using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Identity.Client.Core
{
    internal static class DictionaryExtensions
    {
        public static IDictionary<T, T2> CopyDictionary<T, T2>(this IDictionary<T, T2> input)
        {
            var dict = new Dictionary<T, T2>();
            foreach (var kvp in input)
            {
                dict[kvp.Key] = kvp.Value;
            }

            return dict;
        }
    }
}
