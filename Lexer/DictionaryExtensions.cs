using Lexer.Automaton;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexer
{
    /// <summary>
    /// Extensions for dictionaries.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Merges THIS dictionary with other dictionaries and returns a new dictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="this"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<TKey, TValue> MergeWith<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> @this, params IReadOnlyDictionary<TKey, TValue>[] others)
        {
            var keyValues = new[] { @this }.Concat(others).SelectMany(d => d);
            return keyValues.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        /// <summary>
        /// Alters a value in a dictionary. If not present it executes the action on a given default value.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <param name="default"></param>
        public static void AlterValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> action, TValue @default = default(TValue))
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, action(@default));
            }
            else
            {
                dictionary[key] = action(dictionary[key]);
            }
        }

        /// <summary>
        /// Gets a value for the given key, or if the key does not exist, does insert a default value.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static TValue GetValueOrInsertedDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue @default = default(TValue))
        {
            if (!dictionary.TryGetValue(key, out TValue result))
            {
                dictionary[key] = result = @default;
            }

            return result;
        }

        /// <summary>
        /// Gets a value for the given key, or if the key does not exist, does insert a default value lazyly.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static TValue GetValueOrInsertedLazyDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> @default)
        {
            if (!dictionary.TryGetValue(key, out TValue result))
            {
                dictionary[key] = result = @default();
            }

            return result;
        }

        /// <summary>
        /// Returns the value for an existing key, or, if not existing, a default value. 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="_this"></param>
        /// <param name="key"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> _this, TKey key, TValue @default = default(TValue))
        {
            if (_this.TryGetValue(key, out TValue value))
            {
                return value;
            }

            return @default;
        }

        public static IEnumerable<TKey> GetKeys<TKey, TValue>(this ILookup<TKey, TValue> @this)
        {
            foreach (var group in @this)
                yield return group.Key;
        }
    }
}
