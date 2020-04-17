namespace Merchello.Web.PropertyConverters
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Web.Models;
    using Merchello.Web.Models.Ui.Rendering;

    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Core.PropertyEditors;

    /// <summary>
    /// A property value converter for the Merchello.ProductcollectionPicker.
    /// </summary>
    [PropertyValueType(typeof(IProductCollection))]
    [PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.Content)]
    public class ProductCollectionValueConverter : PropertyValueConverterBase
    {
        /// <summary>
        /// The property editor aliases that to be associated with this converter
        /// </summary>
        private static readonly string[] EditorAliases = { "Merchello.ProductCollectionPicker" };

        /// <summary>
        /// The is converter.
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
        /// The convert data to source.
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
            
            if (string.IsNullOrEmpty(source.ToString()))
                return null;

            try
            {
                var key = new Guid(source.ToString());
                var collection = MerchelloContext.Current.Services.EntityCollectionService.GetByKey(key);
                return collection != null ? new ProductCollection(collection) : null;
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<ProductCollectionValueConverter>("Failed to Convert ProductCollection property", ex);
                return null;
            }
        }
    }
}