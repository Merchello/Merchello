namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// Represents a DTO object used to represent line items.
    /// </summary>
    /// <remarks>
    /// ItemCache, Invoice, Orders, Shipments
    /// </remarks>
    internal interface ILineItemDto : IExtendedDataDto
    {
        /// <summary>
        /// Gets or sets the line item type field key.
        /// </summary>
        Guid LineItemTfKey { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        string Sku { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        decimal Price { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether exported.
        /// </summary>
        bool Exported { get; set; }     
    }
}