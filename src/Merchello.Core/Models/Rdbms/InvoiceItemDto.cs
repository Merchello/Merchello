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

        [Column("updateDate")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

    }

}