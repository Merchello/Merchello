using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchPayment")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class PaymentDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("invoiceId")]
        [ForeignKey(typeof(InvoiceDto), Name = "FK_merchPayment_merchInvoice", Column = "id")]
        public int InvoiceId { get; set; }

        [Column("customerKey")]
        [ForeignKey(typeof(CustomerDto), Name = "FK_merchPayment_merchCustomer", Column = "pk")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchPaymentCustomer")]
        public Guid CustomerKey { get; set; }

        [Column("memberId")]
        public int? MemberId { get; set; }

        [Column("gatewayAlias")]
        public string GatewayAlias { get; set; }

        [Column("paymentTypeFieldKey")]
        public Guid PaymentTypeFieldKey { get; set; }

        [Column("paymentMethodName")]
        public string PaymentMethodName { get; set; }

        [Column("referenceNumber")]
        public string ReferenceNumber { get; set; }

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

        [ResultColumn]
        public CustomerDto CustomerDto { get; set; }


    }
}