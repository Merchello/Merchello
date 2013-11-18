using System;
using Umbraco.Core.Media;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchTransaction")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class TransactionDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("paymentKey")]
        [ForeignKey(typeof(PaymentDto), Name = "FK_merchTransaction_merchPayment", Column = "pk")]
        public Guid PaymentKey { get; set; }

        [Column("transactionTfKey")]
        public Guid TransactionTfKey { get; set; }

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

        /*
        [ResultColumn]
        public PaymentDto PaymentDto { get; set; }

        [ResultColumn]
        public InvoiceDto InvoiceDto { get; set; }
         * */
    }
}