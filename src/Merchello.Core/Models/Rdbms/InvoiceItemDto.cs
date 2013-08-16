using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchInvoiceItem")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class InvoiceItemDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        //TODO: RSS IndexAttribute - ref NodeDto
        [Column("parentId")]
        [ForeignKey(typeof(InvoiceItemDto))]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchInvoiceItemParent")]
        public int ParentId { get; set; }

        [Column("invoiceId")]
        [ForeignKey(typeof(InvoiceDto), Name = "FK_merchInvoiceItem_merchInvoice", Column = "id")]
        public int InvoiceId { get; set; }
        

        [Column("invoiceItemTypeFieldKey")]
        public Guid InvoiceItemTypeFieldKey { get; set; }

        [Column("sku")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchInvoiceItemSku")]
        public string Sku { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("baseQuantity")]
        public int BaseQuantity { get; set; }

        [Column("unitOfMeasureMultiplier")]
        [Constraint(Default = "1")]
        public int UnitOfMeasureMultiplier { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("exported")]
        public bool Exported { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }

}