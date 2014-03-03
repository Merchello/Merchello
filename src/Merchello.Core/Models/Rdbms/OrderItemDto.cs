using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchOrderItem")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class OrderItemDto : ILineItemDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("orderKey")]
        [ForeignKey(typeof(OrderDto), Name = "FK_merchOrderItem_merchOrder", Column = "pk")]
        public Guid ContainerKey { get; set; }

        [Column("shipmentKey")]
        [ForeignKey(typeof(ShipmentDto), Name = "FK_merchOrderItem_merchShipment", Column = "pk")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid? ShipmentKey { get; set; }
        
        [Column("lineItemTfKey")]
        public Guid LineItemTfKey { get; set; }

        [Column("sku")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchOrderItemSku")]
        public string Sku { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("backOrder")]
        public bool BackOrder { get; set; }

        [Column("exported")]
        public bool Exported { get; set; }

        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }


    }

}