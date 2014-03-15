using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchInvoiceIndex")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class InvoiceIndexDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("invoiceKey")]
        [ForeignKey(typeof(InvoiceDto), Name = "FK_merchInvoiceIndex_merchInvoice", Column = "pk")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_merchInvoiceIndex")]
        public Guid InvoiceKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; } 
    }
}