namespace Merchello.Bazaar.Models
{
    using System;

    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core.Models;

    /// <summary>
    /// The basket line item.
    /// </summary>
    public class BasketLineItem
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the content id.
        /// </summary>
        [Obsolete("Use IProductContent")]
        public int ContentId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        public string Attributes { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        public ExtendedDataCollection ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        public ProductModel Product { get; set; }
    }
}