using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchInvoice")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class InvoiceDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("customerPk")]
        [ForeignKey(typeof(CustomerDto), Name = "FK_merchInvoice_merchCustomer",Column = "pk")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchInvoiceCustomer")]
        public int CustomerId { get; set; }

        [Column("invoiceNumber")]
        [IndexAttribute(IndexTypes.UniqueNonClustered, Name = "IX_merchInvoiceNumber")]
        public string InvoiceNumber { get; set; }

        [Column("invoiceDate")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchInvoiceDate")]
        public DateTime InvoiceDate { get; set; }

        [Column("invoiceStatusId")]
        [ForeignKey(typeof(InvoiceStatusDto), Name = "FK_merchInvoice_merchInvoiceStatus", Column = "id")]
        public int InvoiceStatusId { get; set; }

        [Column("billToName")]
        public string BillToName { get; set; }

        [Column("billToAddress1")]
        public string BillToAddress1 { get; set; }

        [Column("billToAddress2")]
        public string BillToAddress2 { get; set; }

        [Column("billToLocality")]
        public string BillToLocality { get; set; }

        [Column("billToRegion")]
        public string BillToRegion { get; set; }

        [Column("billToPostalCode")]
        public string BillToPostalCode { get; set; }

        [Column("billToCountryCode")]
        public string BillToCountryCode { get; set; }

        [Column("billToEmail")]
        public string BillToEmail { get; set; }

        [Column("billToPhone")]
        public string BillToPhone { get; set; }

        [Column("billToCompany")]
        public string BillToCompany { get; set; }

        [Column("exported")]
        public bool Exported { get; set; }

        [Column("paid")]
        public bool Paid { get; set; }

        [Column("shipped")]
        public bool Shipped { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

        [ResultColumn]
        public CustomerDto CustomerDto { get; set; }

    }
}