namespace Merchello.Core.Models.DetachedContent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Core.Persistence.Factories;

    using Newtonsoft.Json;

    using Umbraco.Core;

    /// <summary>
    /// Represents a serialization helper to ensure consistency in serialization and deserialization of detached
    /// content values.
    /// </summary>
    internal class DetachedContentValuesSerializationHelper
    {
        /// <summary>
        /// Deserializes JSON stored in database fields.
        /// </summary>
        /// <param name="json">
        /// The JSON value stored in the database field.
        /// </param>
        /// <returns>
        /// The collection of key value pairs to be used in an object instance.
        /// </returns>
        public static IEnumerable<KeyValuePair<string, string>> Deserialize(string json)
        {
            try
            {
                return json.IsNullOrWhiteSpace()
                                 ? Enumerable.Empty<KeyValuePair<string, string>>()
                                 : JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(json);
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<ProductVariantDetachedContentFactory>("Failed to deserialize detached content values", ex);
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }
        }

        /// <summary>
        /// Serializes values to JSON to be persisted in the database.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The JSON <see cref="string"/>.
        /// </returns>>
        public static string Serialize(IEnumerable<KeyValuePair<string, string>> values)
        {
            var keyValuePairs = values as KeyValuePair<string, string>[] ?? values.ToArray();

            return keyValuePairs.Any()
                       ? JsonConvert.SerializeObject(keyValuePairs.AsEnumerable())
                       : null;
        }
    }
}