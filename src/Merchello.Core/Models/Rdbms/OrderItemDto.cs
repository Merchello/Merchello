namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchOrderItem" table.
    /// </summary>
    internal class OrderItemDto : LineItemDto, IEntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the order key which represents the container for the line item.
        /// </summary>
        public Guid ContainerKey { get; set; }

        /// <summary>
        /// Gets or sets the shipment key.
        /// </summary>
        [CanBeNull]
        public Guid? ShipmentKey { get; set; }

        /// <summary>
        /// Gets or sets the sku.
        /// </summary>
        public override string Sku { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the order is on back order.
        /// </summary>
        public bool BackOrder { get; set; }
    }
}