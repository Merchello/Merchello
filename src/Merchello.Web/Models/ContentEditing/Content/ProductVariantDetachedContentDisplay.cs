namespace Merchello.Web.Models.ContentEditing.Content
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.ValueConverters;

    using Newtonsoft.Json;

    /// <summary>
    /// The product variant detached content display.
    /// </summary>
    public class ProductVariantDetachedContentDisplay : IHaveDetachedDataValues
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
}