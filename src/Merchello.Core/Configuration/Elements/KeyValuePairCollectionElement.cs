namespace Merchello.Core.Configuration.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Acquired;

    using Merchello.Core.Acquired.Configuration;

    /// <summary>
    /// Represents a configuration element that constructs a collection of key value pairs.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the key
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The type of the value
    /// </typeparam>
    internal class KeyValuePairCollectionElement<TKey, TValue> : RawXmlConfigurationElement
    {
        /// <summary>
        /// Creates a collection of <see cref="KeyValuePair{TKey, TValue}"/> from child elements.
        /// </summary>
        /// <param name="childElementName">
        /// The name of the child element that contains the key and value
        /// </param>
        /// <param name="keyAttributeName">
        /// The configuration attribute used as the key.
        /// </param>
        /// <param name="valueAttributeName">
        /// The configuration attribute used as the value.
        /// </param>
        /// <returns>
        /// The collection of <see cref="KeyValuePair{TKey, TValue}"/> based on configuration attributes.
        /// </returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> GetKeyValuePairs(string childElementName, string keyAttributeName, string valueAttributeName)
        {
            var xpairs = RawXml.Elements(childElementName).ToArray();
            if (!xpairs.Any()) return Enumerable.Empty<KeyValuePair<TKey, TValue>>();

            return
                xpairs.Select(
                    x => Create(
                        x.Attribute("keyAttributeName").Value, 
                        x.Attribute("valueAttributeName").Value));
        }

        /// <summary>
        /// Creates a <see cref="KeyValuePair{TKey, TValue}"/> from configuration element values.
        /// </summary>
        /// <param name="ckey">
        /// The key from the configuration file.
        /// </param>
        /// <param name="cValue">
        /// The value from the configuration file.
        /// </param>
        /// <returns>
        /// The <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if either the key or value could not be converted to type specified.
        /// </exception>
        private KeyValuePair<TKey, TValue> Create(object ckey, object cValue)
        {
            var key = ckey.TryConvertTo<TKey>();
            var value = cValue.TryConvertTo<TValue>();

            if (!key.Success) throw key.Exception;
            if (!value.Success) throw value.Exception;

            
            return new KeyValuePair<TKey, TValue>(key.Result, value.Result);
            
        }
    }
}