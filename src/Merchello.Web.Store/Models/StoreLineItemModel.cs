namespace Merchello.Web.Store.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Models.VirtualContent;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// A model used to represent items in a basket or invoice.
    /// </summary>
    public class StoreLineItemModel : ILineItemModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreLineItemModel"/> class.
        /// </summary>
        public StoreLineItemModel()
        {
            this.CustomerOptionChoices = Enumerable.Empty<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Gets or sets the line item key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        public IProductContent Product { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="LineItemType"/>.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public LineItemType LineItemType { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> CustomerOptionChoices { get; set; }
    }
}