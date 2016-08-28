namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// Defines a DTO object used to represent line items
    /// </summary>
    internal interface ILineItemDto : IExtendedDataDto, IEntityDto
    {
        /// <summary>
        /// Gets or sets the container key.
        /// </summary>
        Guid ContainerKey { get; set; }

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