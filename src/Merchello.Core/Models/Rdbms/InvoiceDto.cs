using System;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchInvoice")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class InvoiceDto
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("customerId")]
        public int CustomerId { get; set; }

        [Column("invoiceNumber")]
        public string InvoiceNumber { get; set; }

        [Column("invoiceDate")]
        public DateTime InvoiceDate { get; set; }

        [Column("invoiceStatusId")]
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

        [Column("billToCompanyName")]
        public string BillToCompanyName { get; set; }

        [Column("exported")]
        public bool Exported { get; set; }

        [Column("paid")]
        public bool Paid { get; set; }

        [Column("shipped")]
        public bool Shipped { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("updateDate")]
        public DateTime UpdateDate { get; set; }

        [Column("createdDate")]
        public DateTime CreatedDate { get; set; }

    }
}