using System;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchInvoiceItem")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class InvoiceItemDto
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("parentId")]
        public int? ParentId { get; set; }

        [Column("invoiceId")]
        public int InvoiceId { get; set; }

        [Column("invoiceItemTypeId")]
        public int InvoiceItemTypeId { get; set; }

        [Column("sku")]
        public string Sku { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("baseQuantity")]
        public int? BaseQuantity { get; set; }

        [Column("unitOfMeasureMultiplier")]
        public int? UnitOfMeasureMultiplier { get; set; }

        [Column("amount")]
        public decimal? Amount { get; set; }

        [Column("exported")]
        public bool Exported { get; set; }

        [Column("updateDate")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

    }

}