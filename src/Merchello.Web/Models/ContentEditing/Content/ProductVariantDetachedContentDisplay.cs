namespace Merchello.Web.Models.ContentEditing.Content
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.ValueConverters;
    using Merchello.Web.Models.VirtualContent;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;

    /// <summary>
    /// The product variant detached content display.
    /// </summary>
    public class ProductVariantDetachedContentDisplay
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantDetachedContentDisplay"/> class.
        /// </summary>
        public ProductVariantDetachedContentDisplay()
        {
            this.ValueConversion = DetachedValuesConversionType.Db;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the detached content type.
        /// </summary>
        public DetachedContentTypeDisplay DetachedContentType { get; set; }
        
        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        public Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the culture name.
        /// </summary>
        public string CultureName { get; set; }

        /// <summary>
        /// Gets or sets the template id.
        /// </summary>
        public int TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the virtual content can be rendered.
        /// </summary>
        public bool CanBeRendered { get; set; }

        /// <summary>
        /// Gets or sets the detached data values.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> DetachedDataValues { get; set; }

        // Some properties use the create and update dates for caching - like ImageCropper

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is for back office for editor.
        /// </summary>
        /// <remarks>
        /// We need this due the ability for developers to override the value returned 
        /// from a property specifically for back office editors and when rendering for the 
        /// front end content we want to use the raw database value instead.
        /// </remarks>
        [JsonIgnore]
        internal DetachedValuesConversionType ValueConversion { get; set; }
    }

    /// <summary>
    /// Utility mapping extensions for <see cref="ProductVariantDetachedContentDisplay"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class ProductVariantDetachedContentDisplayExtensions
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
            return AutoMapper.Mapper.Map<ProductVariantDetachedContentDisplay>(detachedContent);
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
        /// <param name="pvd">
        /// The <see cref="ProductVariantDetachedContentDisplay"/>.
        /// </param>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IPublishedProperty}"/>.
        /// </returns>
        public static IEnumerable<IPublishedProperty> DataValuesAsPublishedProperties(this ProductVariantDetachedContentDisplay pvd, PublishedContentType contentType)
        {
            var properties = new List<IPublishedProperty>();

            foreach (var dcv in pvd.DetachedDataValues)
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
    }
}