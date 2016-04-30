namespace Merchello.Core
{
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
            var stringify = propVal.ToString().Trim();

            return (stringify.StartsWith("{") && stringify.EndsWith("}")) ||
                   (stringify.StartsWith("[") && stringify.EndsWith("]"));
        }
    }
}