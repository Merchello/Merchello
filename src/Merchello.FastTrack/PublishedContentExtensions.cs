namespace Merchello.FastTrack
{
    using System;
    using System.Linq;

    using Umbraco.Core.Models;
    using Umbraco.Web;

    public static class PublishedContentExtensions
    {

        /// <summary>
        /// Gets the data value of a IPublishedContent property.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="propertyAlias">
        /// The property alias.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> data value.
        /// </returns>
        /// <remarks>
        /// This is useful in instances where we want to get around property value converters
        /// </remarks>
        public static object GetDataValue(this IPublishedContent content, string propertyAlias)
        {
            if (!content.HasProperty(propertyAlias) || !content.HasValue(propertyAlias)) return null;

            var property = content.Properties.First(x => x.PropertyTypeAlias == propertyAlias);

            return property.DataValue;
        }

        /// <summary>
        /// The get data value as GUID.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="propertyAlias">
        /// The property alias.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        /// <remarks>
        /// This is specifically used on this site to get around Merchello's default product list view so we 
        /// can have more flexibility in paging category products.
        /// </remarks>
        public static Guid GetDataValueAsGuid(this IPublishedContent content, string propertyAlias)
        {
            var value = content.GetDataValue(propertyAlias);
            if (value == null) return Guid.Empty;
            Guid converted;
            return Guid.TryParse(value.ToString(), out converted) ? converted : Guid.Empty;
        }
    }
}