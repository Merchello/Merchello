using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchAppliedPayment")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class AppliedPaymentDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("paymentKey")]
        [ForeignKey(typeof(PaymentDto), Name = "FK_merchAppliedPayment_merchPayment", Column = "pk")]
        public Guid PaymentKey { get; set; }

        [Column("invoiceKey")]
        [ForeignKey(typeof(InvoiceDto), Name = "FK_merchAppliedPayment_merchInvoice", Column = "pk")]
        public Guid InvoiceKey { get; set; }

        [Column("appliedPaymentTfKey")]
        public Guid AppliedPaymentTfKey { get; set; }

        [Column("description")]
        public string Description { get; set; }

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