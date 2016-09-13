namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchPayment" table.
    /// </summary>
    [TableName("merchPayment")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class PaymentDto : EntityDto, IExtendedDataDto
    {
        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        [Column("customerKey")]
        [ForeignKey(typeof(CustomerDto), Name = "FK_merchPayment_merchCustomer", Column = "pk")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchPaymentCustomer")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid? CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the payment method key.
        /// </summary>
        [Column("paymentMethodKey")]
        [ForeignKey(typeof(PaymentMethodDto), Name = "FK_merchPayment_merchPaymentMethod", Column = "pk")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid? PaymentMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the payment type field key.
        /// </summary>
        [Column("paymentTfKey")]
        public Guid PaymentTfKey { get; set; }

        /// <summary>
        /// Gets or sets the payment method name.
        /// </summary>
        [Column("paymentMethodName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Gets or sets the reference number.
        /// </summary>
        [Column("referenceNumber")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [Column("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether authorized.
        /// </summary>
        [Column("authorized")]
        [Constraint(Default = "1")]
        public bool Authorized { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether collected.
        /// </summary>
        [Column("collected")]
        [Constraint(Default = "1")]
        public bool Collected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the payment has been voided.
        /// </summary>
        [Column("voided")]
        [Constraint(Default = "0")]
        public bool Voided { get; set; }

        /// <summary>
        /// Gets or sets the extended data serialization.
        /// </summary>
        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether exported.
        /// </summary>
        [Column("exported")]
        [Constraint(Default = "0")]
        public bool Exported { get; set; }
    }
}