﻿using System;
using Umbraco.Core.Media;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchAppliedPayment")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class AppliedPaymentDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("paymentId")]
        [ForeignKey(typeof(PaymentDto), Name = "FK_merchAppliedPayment_merchPayment", Column = "id")]
        public int PaymentId { get; set; }

        [Column("invoiceId")]
        [ForeignKey(typeof(InvoiceDto), Name = "FK_merchAppliedPayment_merchInvoice", Column = "id")]
        public int InvoiceId { get; set; }

        [Column("appliedPaymentTypeFieldKey")]
        public Guid AppliedPaymentTypeFieldKey { get; set; }

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