namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The invoice line item display.
    /// </summary>
    public class InvoiceLineItemDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the container key.
        /// </summary>
        public Guid ContainerKey { get; set; }

        /// <summary>
        /// Gets or sets the line item type field key.
        /// </summary>
        public Guid LineItemTfKey { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether exported.
        /// </summary>
        public bool Exported { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }
    }
}