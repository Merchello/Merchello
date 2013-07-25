using System;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchPayment")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class PaymentDto
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("invoiceId")]
        public int InvoiceId { get; set; }

        [Column("customerId")]
        public int CustomerId { get; set; }

        [Column("memberId")]
        public int? MemberId { get; set; }

        [Column("gatewayAlias")]
        public string GatewayAlias { get; set; }

        [Column("paymentMethodName")]
        public string PaymentMethodName { get; set; }

        [Column("referenceNumber")]
        public string ReferenceNumber { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("exported")]
        public bool Exported { get; set; }

        [Column("updateDate")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

    }
}