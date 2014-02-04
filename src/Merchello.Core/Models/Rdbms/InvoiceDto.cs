using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchInvoice")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class InvoiceDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("customerKey")]
        [ForeignKey(typeof(CustomerDto), Name = "FK_merchInvoice_merchCustomer",Column = "pk")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid? CustomerKey { get; set; }

        [Column("invoiceNumber")]
        [IndexAttribute(IndexTypes.UniqueNonClustered, Name = "IX_merchInvoiceNumber")]
        public int InvoiceNumber { get; set; }

        [Column("invoiceDate")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchInvoiceDate")]
        public DateTime InvoiceDate { get; set; }

        [Column("invoiceStatusKey")]
        [ForeignKey(typeof(InvoiceStatusDto), Name = "FK_merchInvoice_merchInvoiceStatus", Column = "pk")]
        public Guid InvoiceStatusKey { get; set; }

        [Column("billToName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string BillToName { get; set; }

        [Column("billToAddress1")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string BillToAddress1 { get; set; }

        [Column("billToAddress2")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string BillToAddress2 { get; set; }

        [Column("billToLocality")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string BillToLocality { get; set; }

        [Column("billToRegion")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string BillToRegion { get; set; }

        [Column("billToPostalCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string BillToPostalCode { get; set; }

        [Column("billToCountryCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string BillToCountryCode { get; set; }

        [Column("billToEmail")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string BillToEmail { get; set; }

        [Column("billToPhone")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string BillToPhone { get; set; }

        [Column("billToCompany")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string BillToCompany { get; set; }

        [Column("exported")]
        public bool Exported { get; set; }

        [Column("paid")]
        public bool Paid { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

        [ResultColumn]
        public InvoiceStatusDto InvoiceStatusDto { get; set; }

        [ResultColumn]
        public CustomerDto CustomerDto { get; set; }

    }
}