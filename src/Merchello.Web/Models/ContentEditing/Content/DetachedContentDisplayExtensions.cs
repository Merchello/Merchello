namespace Merchello.Web.Models.ContentEditing.Content
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using AutoMapper;

    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.ValueConverters;
    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;

    /// <summary>
    /// Utility mapping extensions for <see cref="ProductVariantDetachedContentDisplay"/>.
    /// </summary>
    internal static class DetachedContentDisplayExtensions
    {
        /// <summary>
        /// The to product variant detached content display.
        /// </summary>
        /// <param name="detachedContent">
        /// The detached content.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDetachedContentDisplay"/>.
        /// </returns>
        public static ProductVariantDetachedContentDisplay ToProductVariantDetachedContentDisplay(
            this IProductVariantDetachedContent detachedContent)
        {
            return Mapper.Map<ProductVariantDetachedContentDisplay>(detachedContent);
        }

        /// <summary>
        /// Maps <see cref="ProductVariantDetachedContentDisplay"/> to <see cref="IProductVariantDetachedContent"/>.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="productVariantKey">
        /// The product Variant Key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariantDetachedContent"/>.
        /// </returns>
        public static IProductVariantDetachedContent ToProductVariantDetachedContent(this ProductVariantDetachedContentDisplay display, Guid productVariantKey)
        {
            var contentType = new DetachedContentType(
                display.DetachedContentType.EntityTypeField.TypeKey,
                display.DetachedContentType.UmbContentType.Key)
                                  {
                                      Key = display.DetachedContentType.Key,
                                      Name = display.DetachedContentType.Name,
                                      Description = display.DetachedContentType.Description
                                  };

            // Assign the default template
            var templateId = 0;
            if (display.TemplateId == 0 && display.DetachedContentType.UmbContentType.DefaultTemplateId != 0)
            {
                templateId = display.DetachedContentType.UmbContentType.DefaultTemplateId;
            }

            var pvdc = new ProductVariantDetachedContent(productVariantKey, contentType, display.CultureName, new DetachedDataValuesCollection(display.DetachedDataValues))
                           {                           
                               TemplateId = templateId,
                               Slug = display.Slug,
                               CanBeRendered = display.CanBeRendered
                           };
            if (!display.Key.Equals(Guid.Empty)) pvdc.Key = display.Key;
            return pvdc;
        }

        /// <summary>
        /// The to product variant detached content.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariantDetachedContent"/>.
        /// </returns>
        public static IProductVariantDetachedContent ToProductVariantDetachedContent(
            this ProductVariantDetachedContentDisplay display,
            IProductVariantDetachedContent destination)
        {
            destination.Slug = display.Slug;
            destination.TemplateId = display.TemplateId;
            destination.CanBeRendered = display.CanBeRendered;

            // Find any detached content items that should be removed
            var validPropertyTypeAliases = display.DetachedDataValues.Select(x => x.Key);
            var removers = destination.DetachedDataValues.Where(x => validPropertyTypeAliases.All(y => y != x.Key));
            foreach (var remove in removers)
            {
                destination.DetachedDataValues.RemoveValue(remove.Key);
            }

            foreach (var item in display.DetachedDataValues)
            {
                if (!item.Key.IsNullOrWhiteSpace())
                    destination.DetachedDataValues.AddOrUpdate(item.Key, item.Value, (x, y) => item.Value);
            }

            return destination;
        }

        /// <summary>
        /// The data values as published properties.
        /// </summary>
        /// <param name="container">
        /// The <see cref="ProductVariantDetachedContentDisplay"/>.
        /// </param>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IPublishedProperty}"/>.
        /// </returns>
        public static IEnumerable<IPublishedProperty> DataValuesAsPublishedProperties(this IHaveDetachedDataValues container, PublishedContentType contentType)
        {
            var properties = new List<IPublishedProperty>();

            foreach (var dcv in container.DetachedDataValues)
            {
                var propType = contentType.GetPropertyType(dcv.Key);
                object valObj;
                try
                {
                    valObj = DetachedValuesConverter.Current.ConvertDbForContent(propType, dcv).Value;
                }
                catch
                {
                    valObj = dcv.Value;
                }

                if (propType != null)
                {
                    properties.Add(new DetachedPublishedProperty(propType, valObj));
                }
            }

            return properties;
        }

        /// <summary>
        /// Utility for setting the IsForBackOfficeEditor property.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="conversionType">
        /// The value conversion type.
        /// </param>
        internal static void EnsureValueConversion(this ProductDisplay display, DetachedValuesConversionType conversionType = DetachedValuesConversionType.Db)
        {
            ((ProductDisplayBase)display).EnsureValueConversion(conversionType);
            if (display.ProductVariants.Any())
            {
                foreach (var variant in display.ProductVariants)
                {
                    variant.EnsureValueConversion(conversionType);
                }
            }
        }

        /// <summary>
        /// Utility for setting the IsForBackOfficeEditor property.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="conversionType">
        /// The value conversion type.
        /// </param>
        internal static void EnsureValueConversion(this ProductDisplayBase display, DetachedValuesConversionType conversionType = DetachedValuesConversionType.Db)
        {
            if (display.DetachedContents.Any())
            {
                foreach (var dc in display.DetachedContents)
                {
                    var contentType = DetachedValuesConverter.Current.GetContentTypeByKey(dc.DetachedContentType.UmbContentType.Key);
                    if (dc.ValueConversion != conversionType && contentType != null)
                    {
                            dc.DetachedDataValues = DetachedValuesConverter.Current.Convert(contentType, dc.DetachedDataValues, conversionType);
                            dc.ValueConversion = conversionType;
                    }
                }
            }
        }

        internal static void EnsureValueConversion(this ProductOptionDisplay display, DetachedValuesConversionType conversionType = DetachedValuesConversionType.Db)
        {
            if (display.DetachedContentTypeKey.Equals(Guid.Empty)) return;

            var contentType = new Lazy<IContentType>(() => DetachedValuesConverter.Current.GetContentTypeByKey(display.DetachedContentTypeKey));
            foreach (var choice in display.Choices.Where(x => x.ValueConversion != conversionType))
            {
                if (contentType.Value != null)
                {
                    choice.ValueConversion = conversionType;
                    choice.DetachedDataValues = DetachedValuesConverter.Current.Convert(contentType.Value, choice.DetachedDataValues, conversionType);
                }
                    
            }
        }
    }
}