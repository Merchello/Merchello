﻿using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchOrderItem")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class OrderItemDto : ILineItemDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("orderId")]
        [ForeignKey(typeof(OrderDto), Name = "FK_merchOrderItem_merchOrder", Column = "id")]
        public int ContainerId { get; set; }
        
        [Column("lineItemTfKey")]
        public Guid LineItemTfKey { get; set; }

        [Column("sku")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchOrderItemSku")]
        public string Sku { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

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

        [ResultColumn]
        public OrderDto OrderDto { get; set; }
    }

}