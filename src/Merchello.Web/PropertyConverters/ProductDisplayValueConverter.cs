namespace Merchello.Web.PropertyConverters
{
    using System;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core.Logging;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Core.PropertyEditors;

    /// <summary>
    /// A property value converter for converting <see cref="ProductDisplay"/>.
    /// </summary>
    [PropertyValueType(typeof(ProductDisplay))]
    [PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.Content)]
    [Obsolete("Use IProductContent properties")]
    public class ProductDisplayValueConverter : PropertyValueConverterBase
    {
        /// <summary>
        /// The property editor aliases that to be associated with this converter
        /// </summary>
        private static readonly string[] EditorAliases = { "Merchello.ProductSelector" };

        /// <summary>
        /// Returns a value indication if this is a PropertyValue converter for the current property.
        /// </summary>
        /// <param name="propertyType">
        /// The property type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return !string.IsNullOrEmpty(propertyType.PropertyEditorAlias)
                                          && EditorAliases.Contains(propertyType.PropertyEditorAlias);
        }

        /// <summary>
        /// Converts property data to <see cref="ProductDisplay"/>.
        /// </summary>
        /// <param name="propertyType">
        /// The property type.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="preview">
        /// The preview.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            var merchello = new MerchelloHelper();
            
            if (source == null)
                return null;

            var productKey = source.ToString();

            try
            {
                var key = new Guid(productKey);
                return merchello.Query.Product.GetByKey(key);
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<ProductDisplayValueConverter>("Failed to Convert ProductDisplay property", ex);
                return null;
            }
        }
    }
}