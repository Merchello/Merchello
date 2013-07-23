using System;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchPaymentTransaction")]
    [PrimaryKey("id", autoIncrement = false)]
    [ExplicitColumns]
    public class PaymentTransactionDto
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("paymentId")]
        public int PaymentId { get; set; }

        [Column("type")]
        public int Type { get; set; }

        [Column("description")]
        public string Description { get; set; }

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