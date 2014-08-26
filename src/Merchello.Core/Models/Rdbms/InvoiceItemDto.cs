namespace Merchello.Core.Models.Rdbms
{
    using System;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    [TableName("merchInvoiceItem")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class InvoiceItemDto : ILineItemDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }        

        [Column("invoiceKey")]
        [ForeignKey(typeof(InvoiceDto), Name = "FK_merchInvoiceItem_merchInvoice", Column = "pk")]
        public Guid ContainerKey { get; set; }
        
        [Column("lineItemTfKey")]
        public Guid LineItemTfKey { get; set; }

        [Column("sku")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchInvoiceItemSku")]
        public string Sku { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }        

        [Column("price")]
        public decimal Price { get; set; }

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
        public InvoiceDto InvoiceDto { get; set; }
    }

}