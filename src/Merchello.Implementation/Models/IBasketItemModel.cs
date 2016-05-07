namespace Merchello.Implementation.Models
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// Defines a basket item UI component.
    /// </summary>
    public interface IBasketItemModel
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        string Sku { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        /// <remarks>
        /// This could either by the price or the sale price of a product
        /// </remarks>
        decimal Amount { get; set; }
    }
}