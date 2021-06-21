using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace Mcma.Model
{
    /// <summary>
    /// Base class for all MCMA data model objects, exposing an ExpandoObject-like dynamic interface
    /// </summary>
    public class McmaExpandoObject : IDictionary<string, object>, IDynamicMetaObjectProvider
    {
        private ExpandoObject ExpandoObject { get; } = new ExpandoObject();

        private IDictionary<string, object> PropertyDictionary => ExpandoObject;
        
        /// <summary>
        /// Gets or sets a property using the property's key as an indexer
        /// </summary>
        /// <param name="key"></param>
        public object this[string key] { get => PropertyDictionary[key]; set => PropertyDictionary[key] = value; }

        /// <summary>
        /// Checks if the object has a property with the given name
        /// </summary>
        /// <param name="key">The key of the property to check for</param>
        /// <param name="caseSensitive">Flag indicating if the check should be case-sensitive. Defaults to true.</param>
        /// <returns>True if the object contains a property with the given key; otherwise, false</returns>
        public bool HasProperty(string key, bool caseSensitive = true)
        {
            var dict = GetPropertyDictionary(caseSensitive);
            return dict.ContainsKey(key);
        }

        /// <summary>
        /// Gets the value for the property with the given key
        /// </summary>
        /// <param name="key">The key of the property to get the value for</param>
        /// <param name="caseSensitive">Flag indicating if the check should be case-sensitive. Defaults to true.</param>
        /// <typeparam name="T">The expected type of the property's value</typeparam>
        /// <returns>The value of the property</returns>
        /// <exception cref="InvalidCastException">Thrown when the type of the value of the property does not match expected type T</exception>
        public T Get<T>(string key, bool caseSensitive = true)
        {
            var dict = GetPropertyDictionary(caseSensitive);
            return dict.ContainsKey(key) ? (T)dict[key] : default;
        }

        /// <summary>
        /// Tries to get the value for a given property. If the property is not found on the object, it's set to the default value for type T.
        /// </summary>
        /// <param name="key">The key of the property to get the value for</param>
        /// <param name="caseSensitive">Flag indicating if the check should be case-sensitive. Defaults to true.</param>
        /// <typeparam name="T">The expected type of the property's value</typeparam>
        /// <returns>The value of the property</returns>
        /// <exception cref="InvalidCastException">Thrown when the type of the value of the property does not match expected type T</exception>
        public T GetOrAdd<T>(string key, bool caseSensitive = true) where T : new()
            => TryGet<T>(key, false, out var val) ? val : Set(key, new T());

        /// <summary>
        /// Tries to get the value for a given property. If the property is not found on the object, it's set to the default value for type T.
        /// </summary>
        /// <param name="key">The key of the property to get the value for</param>
        /// <param name="value">The value of the property, if found</param>
        /// <typeparam name="T">The expected type of the property's value</typeparam>
        /// <returns>True if the property was found on the object; otherwise false</returns>
        /// <exception cref="InvalidCastException">Thrown when the type of the value of the property does not match expected type T</exception>
        public bool TryGet<T>(string key, out T value) => TryGet(key, true, out value);

        /// <summary>
        /// Tries to get the value for a given property. If the property is not found on the object, it's set to the default value for type T.
        /// </summary>
        /// <param name="key">The key of the property to get the value for</param>
        /// <param name="caseSensitive">Flag indicating if the check should be case-sensitive. Defaults to true.</param>
        /// <param name="value">The value of the property, if found</param>
        /// <typeparam name="T">The expected type of the property's value</typeparam>
        /// <returns>The value of the property</returns>
        /// <exception cref="InvalidCastException">Thrown when the type of the value of the property does not match expected type T</exception>
        public bool TryGet<T>(string key, bool caseSensitive, out T value)
        {
            var dict = GetPropertyDictionary(caseSensitive);

            value = default;
            
            if (!dict.ContainsKey(key))
                return false;
            
            value = (T)dict[key];
            return true;
        }

        /// <summary>
        /// Sets the value of a property
        /// </summary>
        /// <param name="key">The key of the property to set</param>
        /// <param name="value">The value to set for the property</param>
        /// <typeparam name="T">The type of the property's value</typeparam>
        /// <returns>The value stored on the object</returns>
        public T Set<T>(string key, T value) => (T)(this[key] = value);

        private IDictionary<string, object> GetPropertyDictionary(bool caseSensitive)
            => caseSensitive ? PropertyDictionary : new Dictionary<string, object>(PropertyDictionary, StringComparer.OrdinalIgnoreCase);

        #region Dictionary & Dynamic Implementations

        ICollection<string> IDictionary<string, object>.Keys => PropertyDictionary.Keys;

        ICollection<object> IDictionary<string, object>.Values => PropertyDictionary.Values;

        int ICollection<KeyValuePair<string, object>>.Count => PropertyDictionary.Count;

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => PropertyDictionary.IsReadOnly;

        void IDictionary<string, object>.Add(string key, object value) => PropertyDictionary.Add(key, value);

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item) => PropertyDictionary.Add(item);
        
        void ICollection<KeyValuePair<string, object>>.Clear() => PropertyDictionary.Clear();

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item) => PropertyDictionary.Contains(item);

        bool IDictionary<string, object>.ContainsKey(string key) => PropertyDictionary.ContainsKey(key);

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => PropertyDictionary.CopyTo(array, arrayIndex);

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => PropertyDictionary.GetEnumerator();

        bool IDictionary<string, object>.Remove(string key) => PropertyDictionary.Remove(key);

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) => PropertyDictionary.Remove(item);

        bool IDictionary<string, object>.TryGetValue(string key, out object value) => PropertyDictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => PropertyDictionary.GetEnumerator();

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter) => ((IDynamicMetaObjectProvider)ExpandoObject).GetMetaObject(parameter);

        #endregion
    }
}