namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.ValueConverters;
    using Merchello.Web.Models.ContentEditing.Content;

    using Newtonsoft.Json;

    /// <summary>
    /// The product attribute display.
    /// </summary>
    public class ProductAttributeDisplay //: IHaveDetachedDataValues
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductAttributeDisplay"/> class.
        /// </summary>
        public ProductAttributeDisplay()
        {
            DetachedDataValues = Enumerable.Empty<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the option key.
        /// </summary>
        public Guid OptionKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the detached data values.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> DetachedDataValues { get; set; }

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

        /// <summary>
        /// Gets or sets a value indicating whether is default choice.
        /// </summary>
        public bool IsDefaultChoice { get; set; }
    }
}
