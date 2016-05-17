namespace Merchello.Core
{
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Utility to help with JSON serialization.
    /// </summary>
    internal static class JsonHelper
    {
        /// <summary>
        /// Simple check to guess if a property value is a JSON string
        /// </summary>
        /// <param name="propVal">
        /// The prop val.
        /// </param>
        /// <returns>
        /// The guess.
        /// </returns>
        internal static bool IsJsonObject(object propVal)
        {
            var stringify = propVal.ToString();

            return IsJObject(stringify) ||
                   IsJArray(stringify);
        }

        internal static bool IsJObject(string rawVal)
        {
            var stringify = rawVal.Trim();
            return stringify.StartsWith("{") && stringify.EndsWith("}");
        }

        internal static bool IsJArray(string rawVal)
        {
            var stringify = rawVal.Trim();
            return stringify.StartsWith("[") && stringify.EndsWith("]");
        }

        internal static object GetJsonObject(string rawVal)
        {
            if (!IsJsonObject(rawVal)) return rawVal;

            return IsJObject(rawVal)
                       ? JsonConvert.DeserializeObject(rawVal)
                       : JsonConvert.DeserializeObject<IEnumerable<object>>(rawVal);
        }
        
        
    }
}