namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchOrderItem" table.
    /// </summary>
    [TableName("merchOrderItem")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class OrderItemDto : LineItemDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("pk")]
        [Constraint(Default = "newid()")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the order key which represents the container for the line item.
        /// </summary>
        [Column("orderKey")]
        [ForeignKey(typeof(OrderDto), Name = "FK_merchOrderItem_merchOrder", Column = "pk")]
        public override Guid ContainerKey { get; set; }

        /// <summary>
        /// Gets or sets the shipment key.
        /// </summary>
        [Column("shipmentKey")]
        [ForeignKey(typeof(ShipmentDto), Name = "FK_merchOrderItem_merchShipment", Column = "pk")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid? ShipmentKey { get; set; }

        /// <summary>
        /// Gets or sets the sku.
        /// </summary>
        [Column("sku")]
        [Index(IndexTypes.NonClustered, Name = "IX_merchOrderItemSku")]
        public override string Sku { get; set; }
    }
}