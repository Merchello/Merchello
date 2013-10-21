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

        [Column("customerId")]
        [ForeignKey(typeof(CustomerDto), Name = "FK_merchPayment_merchCustomer", Column = "id")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchPaymentCustomer")]
        public int CustomerId { get; set; }
        
        [Column("providerKey")]
        public Guid ProviderKey { get; set; }

        [Column("paymentTfKey")]
        public Guid PaymentTfKey { get; set; }

        [Column("paymentMethodName")]
        public string PaymentMethodName { get; set; }

        [Column("referenceNumber")]
        public string ReferenceNumber { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("authorized")]
        [Constraint(Default = "1")]
        public bool Authorized { get; set; }

        [Column("collected")]
        [Constraint(Default = "1")]
        public bool Collected { get; set; }

        [Column("exported")]
        [Constraint(Default = "0")]
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