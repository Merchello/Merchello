using System;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchCustomer")]
    [PrimaryKey("id", autoIncrement = false)]
    [ExplicitColumns]
    public class CustomerDto
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("memberId")]
        public int? MemberId { get; set; }

        [Column("firstName")]
        public string FirstName { get; set; }

        [Column("lastName")]
        public string LastName { get; set; }

        [Column("totalInvoiced")]
        public decimal TotalInvoiced { get; set; }

        [Column("totalPayments")]
        public decimal TotalPayments { get; set; }

        [Column("lastPaymentDate")]
        public DateTime LastPaymentDate { get; set; }

    }
}