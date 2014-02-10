using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchOrder")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class OrderDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("invoiceKey")]
        [ForeignKey(typeof(InvoiceDto), Name = "FK_merchOrder_merchInvoice",Column = "pk")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid InvoiceKey { get; set; }

        [Column("orderNumber")]
        [IndexAttribute(IndexTypes.UniqueNonClustered, Name = "IX_merchOrderNumber")]
        public string OrderNumber { get; set; }

        [Column("orderDate")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchOrderDate")]
        public DateTime OrdereDate { get; set; }

        [Column("orderStatusKey")]
        [ForeignKey(typeof(OrderStatusDto), Name = "FK_merchOrder_merchOrderStatus", Column = "pk")]
        public Guid OrderStatusKey { get; set; }

        [Column("versionKey")]
        [Constraint(Default = "newid()")]
        public Guid VersionKey { get; set; }

        [Column("exported")]
        public bool Exported { get; set; }
      
        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

        [ResultColumn]
        public OrderStatusDto OrderStatusDto { get; set; }

        [ResultColumn]
        public CustomerDto CustomerDto { get; set; }

    }
}