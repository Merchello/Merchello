namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.ValueConverters;
    using Merchello.Web.Models.ContentEditing.Content;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Workflow.CustomerItemCache;

    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;

    /// <summary>
    /// The product mapping extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Suppression is OK here.")]
    public static class ProductDisplayExtensions
    {

        #region IProduct

        /// <summary>
        /// Maps a <see cref="ProductDisplay"/> to <see cref="IProduct"/>
        /// </summary>
        /// <param name="productDisplay">
        /// The product display.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="languages">
        /// Valid languages defined in Umbraco
        /// </param>
        /// <returns>
        /// The <see cref="IProduct"/>.
        /// </returns>
        internal static IProduct ToProduct(this ProductDisplay productDisplay, IProduct destination, ILanguage[] languages = null)
        {
            if (productDisplay.Key != Guid.Empty)
            {
                destination.Key = productDisplay.Key;
            }

            productDisplay.CatalogInventories = productDisplay.CatalogInventories ?? Enumerable.Empty<CatalogInventoryDisplay>();
            productDisplay.ProductOptions = productDisplay.ProductOptions ?? Enumerable.Empty<ProductOptionDisplay>();

            destination.Name = productDisplay.Name;
            destination.Sku = productDisplay.Sku;
            destination.Price = productDisplay.Price;
            destination.CostOfGoods = productDisplay.CostOfGoods;
            destination.SalePrice = productDisplay.SalePrice;
            destination.OnSale = productDisplay.OnSale;
            destination.Manufacturer = productDisplay.Manufacturer;
            destination.ManufacturerModelNumber = productDisplay.ManufacturerModelNumber;
            destination.Weight = productDisplay.Weight;
            destination.Length = productDisplay.Length;
            destination.Width = productDisplay.Width;
            destination.Height = productDisplay.Height;
            destination.Barcode = productDisplay.Barcode;
            destination.Available = productDisplay.Available;
            destination.TrackInventory = productDisplay.TrackInventory;
            destination.OutOfStockPurchase = productDisplay.OutOfStockPurchase;
            destination.Taxable = productDisplay.Taxable;
            destination.Shippable = productDisplay.Shippable;
            destination.Download = productDisplay.Download;
            destination.DownloadMediaId = productDisplay.DownloadMediaId;

            // We need to refactor the CatalogInventories to not be immutable if we are
            // going to need to do operations like this.  In the UI, the user "unchecks" a catalog that was
            // previously checked - so we need to remove it.
            var deletedCatalogKeys =
                destination.CatalogInventories.Where(
                    x => !productDisplay.CatalogInventories.Select(ci => ci.CatalogKey).Contains(x.CatalogKey)).Select(x => x.CatalogKey).ToArray();
            foreach (var deletedCatalogKey in deletedCatalogKeys)
            {
                ((Product)destination).MasterVariant.RemoveFromCatalogInventory(deletedCatalogKey);
            }

            foreach (var catalogInventory in productDisplay.CatalogInventories)
            {
                var catInv = destination.CatalogInventories.FirstOrDefault(x => x.CatalogKey == catalogInventory.CatalogKey);

                if (catInv != null)
                {
                    var destinationCatalogInventory = catInv;

                    destinationCatalogInventory = catalogInventory.ToCatalogInventory(destinationCatalogInventory);
                }
                else
                {
                    //// Add to a new catalog
                    ((Product)destination).MasterVariant.AddToCatalogInventory(
                        new CatalogInventory(catalogInventory.CatalogKey, catalogInventory.ProductVariantKey)
                        {
                            Location = catalogInventory.Location,
                            Count = catalogInventory.Count,
                            LowCount = catalogInventory.LowCount
                        });
                }
            }

            // Fix option deletion here #M-161
            // remove any product options that exist in destination and do not exist in productDisplay
            var removers = destination.ProductOptions.Where(x => !productDisplay.ProductOptions.Select(pd => pd.Key).Contains(x.Key)).Select(x => x.Key).ToList();
            foreach (var remove in removers)
            {
                destination.ProductOptions.RemoveItem(remove);
            }


            foreach (var option in productDisplay.ProductOptions)
            {
                IProductOption destinationProductOption;
                

                if (destination.ProductOptions.Contains(option.Key))
                {
                    destinationProductOption = destination.ProductOptions[option.Key];

                    destinationProductOption = option.ToProductOption(destinationProductOption);
                }
                else
                {
                    destinationProductOption = new ProductOption(option.Name, option.Required);

                    destinationProductOption = option.ToProductOption(destinationProductOption);
                }

                destination.ProductOptions.Add(destinationProductOption);
            }
            
            destination.AddOrUpdateDetachedContent(productDisplay);
             
            return destination;
        }

        ///// <summary>
        ///// Maps a <see cref="ProductDisplay"/> to <see cref="IProduct"/>
        ///// </summary>
        ///// <param name="productDisplay">
        ///// The product display.
        ///// </param>
        ///// <param name="name">
        ///// The name.
        ///// </param>
        ///// <param name="sku">
        ///// The SKU.
        ///// </param>
        ///// <param name="price">
        ///// The price.
        ///// </param>
        ///// <returns>
        ///// The <see cref="IProduct"/>.
        ///// </returns>
        //internal static IProduct ToProduct(this ProductDisplay productDisplay, string name, string sku, decimal price)
        //{
        //    var destination = MerchelloContext.Current.Services.ProductService.CreateProduct(name, sku, price);
        //    return ToProduct(productDisplay, destination);
        //}

        #endregion

        #region ProductDisplay

        /// <summary>
        /// Gets the <see cref="ProductVariantDisplay"/> with matching with attributes from the product.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="optionChoices">
        /// The option choices.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>        
        public static ProductVariantDisplay GetProductVariantDisplayWithAttributes(this ProductDisplay product, Guid[] optionChoices)
        {
            return
                product.ProductVariants.FirstOrDefault(
                    x =>
                    x.Attributes.Count() == optionChoices.Count()
                    && optionChoices.All(key => x.Attributes.FirstOrDefault(att => att.Key == key) != null));
        }

        /// <summary>
        /// Maps a <see cref="ProductDisplay"/> to <see cref="ProductVariantDisplay"/>.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        /// <remarks>
        /// Used for adding items to item caches in <see cref="CustomerItemCacheBase"/>
        /// </remarks>
        public static ProductVariantDisplay AsMasterVariantDisplay(this ProductDisplay product)
        {
            return AutoMapper.Mapper.Map<ProductVariantDisplay>(product);
        }

        /// <summary>
        /// Maps a <see cref="IProduct"/> to <see cref="ProductDisplay"/>.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="conversionType">
        /// The detached value conversion type.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        public static ProductDisplay ToProductDisplay(this IProduct product, DetachedValuesConversionType conversionType = DetachedValuesConversionType.Db)
        {
            var productDisplay = AutoMapper.Mapper.Map<ProductDisplay>(product);
            productDisplay.EnsureValueConversion(conversionType);
            return productDisplay;
        }        
               
        #endregion


        #region IProductAttribute

        /// <summary>
        /// The to product attribute.
        /// </summary>
        /// <param name="productAttributeDisplay">
        /// The product attribute display.
        /// </param>
        /// <param name="destinationProductAttribute">
        /// The destination product attribute.
        /// </param>
        /// <returns>
        /// The <see cref="IProductAttribute"/>.
        /// </returns>
        internal static IProductAttribute ToProductAttribute(this ProductAttributeDisplay productAttributeDisplay, IProductAttribute destinationProductAttribute)
        {
            if (productAttributeDisplay.Key != Guid.Empty)
            {
                destinationProductAttribute.Key = productAttributeDisplay.Key;
            }

            var validPropertyTypeAliases = productAttributeDisplay.DetachedDataValues.Select(x => x.Key);
            var removeAtts = destinationProductAttribute.DetachedDataValues.Where(x => validPropertyTypeAliases.All(y => y != x.Key));
            foreach (var remove in removeAtts)
            {
                destinationProductAttribute.DetachedDataValues.RemoveValue(remove.Key);
            }

            foreach (var item in productAttributeDisplay.DetachedDataValues)
            {
                if (!item.Key.IsNullOrWhiteSpace())
                    destinationProductAttribute.DetachedDataValues.AddOrUpdate(item.Key, item.Value, (x, y) => item.Value);
            }


            destinationProductAttribute.Name = productAttributeDisplay.Name;
            destinationProductAttribute.Sku = productAttributeDisplay.Sku;
            destinationProductAttribute.OptionKey = productAttributeDisplay.OptionKey;
            destinationProductAttribute.SortOrder = productAttributeDisplay.SortOrder;

            return destinationProductAttribute;
        }

        /// <summary>
        /// Maps <see cref="IProductAttribute"/> to <see cref="ProductAttributeDisplay"/>.
        /// </summary>
        /// <param name="productAttribute">
        /// The product attribute.
        /// </param>
        /// <returns>
        /// The <see cref="ProductAttributeDisplay"/>.
        /// </returns>
        internal static ProductAttributeDisplay ToProductAttributeDisplay(this IProductAttribute productAttribute)
        {
            return productAttribute.ToProductAttributeDisplay(null);
        }

        internal static ProductAttributeDisplay ToProductAttributeDisplay(this IProductAttribute productAttribute, IContentType contentType, DetachedValuesConversionType conversionType = DetachedValuesConversionType.Db)
        {
            var display = AutoMapper.Mapper.Map<ProductAttributeDisplay>(productAttribute);
            if (contentType == null) return display;
            display.EnsureValueConversion(contentType, conversionType);
            return display;
        }


        #endregion

        #region IProductOption

        /// <summary>
        /// The to product option.
        /// </summary>
        /// <param name="productOptionDisplay">
        /// The product option display.
        /// </param>
        /// <param name="destination">
        /// The destination product option.
        /// </param>
        /// <returns>
        /// The <see cref="IProductOption"/>.
        /// </returns>
        internal static IProductOption ToProductOption(this ProductOptionDisplay productOptionDisplay, IProductOption destination)
        {
            if (productOptionDisplay.Key != Guid.Empty)
            {
                destination.Key = productOptionDisplay.Key;
            }

            destination.Name = productOptionDisplay.Name;
            destination.Required = productOptionDisplay.Required;
            destination.SortOrder = productOptionDisplay.SortOrder;
            destination.Shared = productOptionDisplay.Shared;
            destination.UiOption = productOptionDisplay.UiOption;
            destination.UseName = productOptionDisplay.UseName.IsNullOrWhiteSpace()
                                      ? productOptionDisplay.Name
                                      : productOptionDisplay.UseName;

            if (!productOptionDisplay.DetachedContentTypeKey.Equals(Guid.Empty))
            {
                destination.DetachedContentTypeKey = productOptionDisplay.DetachedContentTypeKey;
            }
            else
            {
                destination.DetachedContentTypeKey = null;
            }
            

            // Fix with option deletion here #M-161 #M-150
            // remove any product choices that exist in destination and do not exist in productDisplay
            var removers = destination.Choices.Where(x => !productOptionDisplay.Choices.Select(pd => pd.Key).Contains(x.Key)).Select(x => x.Key).ToArray();
            foreach (var remove in removers)
            {
                destination.Choices.RemoveItem(remove);
            }

            foreach (var choice in productOptionDisplay.Choices)
            {
                // Sets the sku if it is empty - fixes M-170
                // http://issues.merchello.com/youtrack/issue/M-170
                if (string.IsNullOrEmpty(choice.Sku))
                {
                    choice.Sku = Regex.Replace(choice.Name, "[^0-9a-zA-Z]+", "");
                }

                IProductAttribute destinationProductAttribute;

                if (destination.Choices.IndexOfKey(choice.Key) >= 0)
                {
                    destinationProductAttribute = destination.Choices[choice.Key];

                    destinationProductAttribute = choice.ToProductAttribute(destinationProductAttribute);
                }
                else
                {
                    destinationProductAttribute = new ProductAttribute(choice.Name, choice.Sku);

                    destinationProductAttribute = choice.ToProductAttribute(destinationProductAttribute);
                }

                destinationProductAttribute.Name = choice.Name;
                destinationProductAttribute.SortOrder = choice.SortOrder;
                destinationProductAttribute.IsDefaultChoice = choice.IsDefaultChoice;
                destination.Choices.Add(destinationProductAttribute);
            }

            return destination;
        }

        /// <summary>
        /// The to product option display.
        /// </summary>
        /// <param name="productOption">
        /// The product option.
        /// </param>
        /// <param name="conversionType">
        /// The property editor conversion type.
        /// </param>
        /// <returns>
        /// The <see cref="ProductOptionDisplay"/>.
        /// </returns>
        internal static ProductOptionDisplay ToProductOptionDisplay(this IProductOption productOption, DetachedValuesConversionType conversionType = DetachedValuesConversionType.Db)
        {
            var display = AutoMapper.Mapper.Map<ProductOptionDisplay>(productOption);
            display.EnsureValueConversion(conversionType);
            display.Choices = display.Choices.OrderBy(x => x.SortOrder);
            return display;
        }

        #endregion

        #region IProductVariantDetachedContent

        /// <summary>
        /// The product variants as product variant content.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="optionContentTypes">
        /// The option Content Types.
        /// </param>
        /// <param name="optionWrappers">
        /// Product option content wrappers.
        /// </param>
        /// <param name="cultureName">
        /// The cultureName
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductVariantContent}"/>.
        /// </returns>
        internal static IEnumerable<IProductVariantContent> ProductVariantsAsProductVariantContent(this ProductDisplay display, IDictionary<Guid, PublishedContentType> optionContentTypes, IEnumerable<IProductOptionWrapper> optionWrappers, string cultureName, IPublishedContent parent = null)
        {
            var variantContent = new List<IProductVariantContent>();


            // ReSharper disable once LoopCanBeConvertedToQuery
            var optionWrapperArray = optionWrappers as IProductOptionWrapper[] ?? optionWrappers.ToArray();
            foreach (var variant in display.ProductVariants)
            {
                var contentType = variant.DetachedContents.Any()
                                      ? PublishedContentType.Get(
                                          PublishedItemType.Content,
                                          variant.DetachedContentForCulture(cultureName).DetachedContentType.UmbContentType.Alias)
                                      : null;

                var attributes = new List<IProductAttributeContent>();
                foreach (var o in optionWrapperArray)
                {
                    var att = o.Choices.FirstOrDefault(x => variant.Attributes.Select(y => y.Key).Contains(x.Key));
                    if (att != null) attributes.Add(att);
                }

                variantContent.Add(new ProductVariantContent(variant, contentType, optionContentTypes, attributes, cultureName, parent));
            }

            return variantContent;
        }

        /// <summary>
        /// The product option as product option wrapper.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="parent">
        /// The parent node
        /// </param>
        /// <param name="optionContentTypes">
        /// The option content types.
        /// </param>
        /// <returns>
        /// The <see cref="IProductOptionWrapper"/>.
        /// </returns>
        internal static IProductOptionWrapper ProductOptionAsProductOptionWrapper(this ProductOptionDisplay display, IPublishedContent parent, IDictionary<Guid, PublishedContentType> optionContentTypes)
        {
            // Find the associated content type if it exists
            var contentType = optionContentTypes.ContainsKey(display.DetachedContentTypeKey) ?
                optionContentTypes[display.DetachedContentTypeKey] :
                null;

            // This is a hack for the special case when HasProperty and HasValue extensions are called
            // and a content type is not assigned. - so we will default to the product content type
            // if there is none.  The detachedDataValues collection should be empty -
            var usesDefault = contentType == null;
            var ct = usesDefault ? parent.ContentType : contentType;

            var pow = new ProductOptionWrapper(display, parent, ct, usesDefault);
            return pow;
        }

        /// <summary>
        /// Returns a value indicating whether or not this object can be rendered as virtual content.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasVirtualContent(this ProductDisplay display)
        {
            return display.Available && display.DetachedContents.Any(x => x.CanBeRendered);
        }

        /// <summary>
        /// Creates <see cref="IProductContent"/> from the display object.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        [Obsolete("This method will be removed in version 3.0.0")]
        public static IProductContent AsProductContent(this ProductDisplay display)
        {
            return display.AsProductContent(new ProductContentFactory());
        }

        /// <summary>
        /// Creates <see cref="IProductContent"/> from the display object.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        [Obsolete("This method will be removed in version 3.0.0")]
        public static IProductContent AsProductContent(this ProductDisplay display, ProductContentFactory factory)
        {
            if (!display.HasVirtualContent()) return null;
            return factory.BuildContent(display);
        }

        /// <summary>
        /// Gets the default slug.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string GetDefaultSlug(this ProductDisplayBase display)
        {
            return PathHelper.ConvertToSlug(string.Format("{0}-{1}", display.Name, display.Sku));
        }

        /// <summary>
        /// The slug.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string Slug(this ProductDisplayBase display, string cultureName)
        {
            var defaultSlug = display.GetDefaultSlug();
            if (!display.DetachedContents.Any()) return defaultSlug;

            var dc = display.DetachedContentForCulture(cultureName);

            return dc.Slug.IsNullOrWhiteSpace() ? defaultSlug : dc.Slug;
        }

        /// <summary>
        /// The template id.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal static int TemplateId(this ProductDisplayBase display, string cultureName)
        {
            if (!display.DetachedContents.Any()) return 0;

            var dc = display.DetachedContentForCulture(cultureName);

            return dc == null ? 0 : dc.TemplateId;
        }

        /// <summary>
        /// Gets the <see cref="ProductVariantDetachedContentDisplay"/> for a given culture.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDetachedContentDisplay"/>.
        /// </returns>
        internal static ProductVariantDetachedContentDisplay DetachedContentForCulture(this ProductDisplayBase display, string cultureName)
        {
            return display.DetachedContents.Any()
                       ? display.DetachedContents.FirstOrDefault(x => x.CultureName == cultureName)
                       : null;
        }

        /// <summary>
        /// Adds or updates <see cref="IProductVariantDetachedContent"/>.
        /// </summary>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="languages">
        /// A list of languages configured in Umbraco.
        /// </param>
        internal static void AddOrUpdateDetachedContent(this IProductBase destination, ProductDisplayBase display, ILanguage[] languages = null)
        {
            if (destination.DetachedContents.Any())
            {
                // detached content
                // TODO BUG this is not identifying the language to be removed.
                var removedLanguages =
                    destination.DetachedContents.Where(
                        x => !display.DetachedContents.Select(y => y.CultureName).Contains(x.CultureName));

                foreach (var lang in removedLanguages)
                {
                    destination.DetachedContents.RemoveItem(lang.CultureName);
                }
            }


            foreach (var detachedContent in display.DetachedContents.ToArray())
            {
                if (destination.DetachedContents.Contains(detachedContent.CultureName))
                {
                    var destContent = destination.DetachedContents[detachedContent.CultureName];
                    detachedContent.ToProductVariantDetachedContent(destContent);
                }
                else
                {
                    var variant = display.GetType().IsAssignableFrom(typeof(ProductDisplay))
                                      ? ((ProductDisplay)display).AsMasterVariantDisplay()
                                      : (ProductVariantDisplay)display;
                    destination.DetachedContents.Add(detachedContent.ToProductVariantDetachedContent(variant.Key));
                }
            }         
        }

        #endregion

        #region IProductVariant

        internal static ProductVariantDisplay ToProductVariantDisplay(this IProductVariant productVariant, DetachedValuesConversionType conversionType = DetachedValuesConversionType.Db)
        {            
            var display = AutoMapper.Mapper.Map<ProductVariantDisplay>(productVariant);
            display.EnsureValueConversion(conversionType);
            return display;
        }

        internal static IProductVariant ToProductVariant(this ProductVariantDisplay productVariantDisplay, IProductVariant destination)
        {
            if (productVariantDisplay.Key != Guid.Empty)
            {
                destination.Key = productVariantDisplay.Key;
            }
            if( !String.IsNullOrEmpty(productVariantDisplay.Name) )
            {
                destination.Name = productVariantDisplay.Name;
            }
            if( !String.IsNullOrEmpty(productVariantDisplay.Sku) )
            {
                destination.Sku = productVariantDisplay.Sku;
            }
            destination.Price = productVariantDisplay.Price;
            destination.CostOfGoods = productVariantDisplay.CostOfGoods;
            destination.SalePrice = productVariantDisplay.SalePrice;
            destination.OnSale = productVariantDisplay.OnSale;
            destination.Manufacturer = productVariantDisplay.Manufacturer;
            destination.ManufacturerModelNumber = productVariantDisplay.ManufacturerModelNumber;
            destination.Weight = productVariantDisplay.Weight;
            destination.Length = productVariantDisplay.Length;
            destination.Width = productVariantDisplay.Width;
            destination.Height = productVariantDisplay.Height;
            destination.Barcode = productVariantDisplay.Barcode;
            destination.Available = productVariantDisplay.Available;
            destination.TrackInventory = productVariantDisplay.TrackInventory;
            destination.OutOfStockPurchase = productVariantDisplay.OutOfStockPurchase;
            destination.Taxable = productVariantDisplay.Taxable;
            destination.Shippable = productVariantDisplay.Shippable;
            destination.Download = productVariantDisplay.Download;
            destination.DownloadMediaId = productVariantDisplay.DownloadMediaId;

            destination.ProductKey = productVariantDisplay.ProductKey;

            // We need to refactor the CatalogInventories to not be immutable if we are
            // going to need to do operations like this.  In the UI, the user "unchecks" a catalog that was
            // previously checked - so we need to remove it.
            var deletedCatalogKeys =
                destination.CatalogInventories.Where(
                    x => !productVariantDisplay.CatalogInventories.Select(ci => ci.CatalogKey).Contains(x.CatalogKey)).Select(x => x.CatalogKey).ToArray();
            foreach (var deletedCatalogKey in deletedCatalogKeys)
            {
                destination.RemoveFromCatalogInventory(deletedCatalogKey);
            }

            foreach (var catalogInventory in productVariantDisplay.CatalogInventories)
            {
				var catInv = destination.CatalogInventories.FirstOrDefault(x => x.CatalogKey == catalogInventory.CatalogKey);
				
				if (catInv != null)
				{
					var destinationCatalogInventory = catInv;
				
					destinationCatalogInventory = catalogInventory.ToCatalogInventory(destinationCatalogInventory);
				}
				else if (!Guid.Empty.Equals(catalogInventory.CatalogKey) && destination.HasIdentity)
				{
					//// Add to a new catalog
					destination.AddToCatalogInventory(catalogInventory.CatalogKey);
				}
            }

            foreach (var attribute in productVariantDisplay.Attributes)
            {
                IProductAttribute destinationProductAttribute;

                var attr = destination.Attributes.FirstOrDefault(x => x.Key == attribute.Key);
                if (attr != null)
                {
                    destinationProductAttribute = attr;

                    destinationProductAttribute = attribute.ToProductAttribute(destinationProductAttribute);
                }
                else
                {
                    destinationProductAttribute = new ProductAttribute(attribute.Name, attribute.Sku);

                    destinationProductAttribute = attribute.ToProductAttribute(destinationProductAttribute);

                    ProductAttributeCollection variantAttributes = destination.Attributes as ProductAttributeCollection;
                    variantAttributes.Add(destinationProductAttribute);
                }
            }

            destination.AddOrUpdateDetachedContent(productVariantDisplay);

            return destination;
        }

        #endregion
    }
}
