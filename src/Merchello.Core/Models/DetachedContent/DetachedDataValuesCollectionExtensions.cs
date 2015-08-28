namespace Merchello.Core.Models.DetachedContent
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for the <see cref="DetachedDataValuesCollection"/>.
    /// </summary>
    internal static class DetachedDataValuesCollectionExtensions
    {
        /// <summary>
        /// Converts data values collection data into a more easily serializable collection for display classes (back office UI)
        /// </summary>
        /// <param name="dataValues">The <see cref="DetachedDataValuesCollection"/></param>
        /// <returns>An <c>IEnumerable{object}</c></returns>        
        internal static IEnumerable<KeyValuePair<string, string>> AsEnumerable(this DetachedDataValuesCollection dataValues)
        {
            return dataValues.Select(item => new KeyValuePair<string, string>(item.Key, item.Value ?? string.Empty));
        }
    }
}