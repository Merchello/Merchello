namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The DTO for the merchAppliedPayment table.
    /// </summary>
    [TableName("merchAppliedPayment")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class AppliedPaymentDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the payment key.
        /// </summary>
        [Column("paymentKey")]
        [ForeignKey(typeof(PaymentDto), Name = "FK_merchAppliedPayment_merchPayment", Column = "pk")]
        public Guid PaymentKey { get; set; }

        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        [Column("invoiceKey")]
        [ForeignKey(typeof(InvoiceDto), Name = "FK_merchAppliedPayment_merchInvoice", Column = "pk")]
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the applied payment type field key.
        /// </summary>
        [Column("appliedPaymentTfKey")]
        public Guid AppliedPaymentTfKey { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column("description")]
        [Length(500)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [Column("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether exported.
        /// </summary>
        [Column("exported")]
        public bool Exported { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}