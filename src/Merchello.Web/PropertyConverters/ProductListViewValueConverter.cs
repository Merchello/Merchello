namespace Merchello.Web.PropertyConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Models.Ui.Rendering;
    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Core;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Core.PropertyEditors;

    /// <summary>
    /// The product static collection value converter.
    /// </summary>
    [PropertyValueType(typeof(ProductContentListView))]
    [PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.Content)]
    public class ProductListViewValueConverter : PropertyValueConverterBase
    {
        /// <summary>
        /// The property editor aliases that to be associated with this converter
        /// </summary>
        private static readonly string[] EditorAliases = { "Merchello.ProductListView" };

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
            var merchello = new MerchelloHelper();
            
            if (source == null)
                return null;

            var collectionKey = source.ToString();

            if (collectionKey.IsNullOrWhiteSpace())
            {
                var defaultCollection = merchello.Query.Product.TypedProductContentSearch(1, 10);
                    
                return new ProductContentListView(Guid.Empty, defaultCollection);
            }

            try
            {
                var key = new Guid(collectionKey);
                return new ProductContentListView(key, merchello.Query.Product.TypedProductContentFromCollection(key));
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<ProductDisplayValueConverter>("Failed to Convert ProductDisplay property", ex);
                return null;
            }
        }
    }
}