namespace Merchello.Web.Models.Payments
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Web.Models.ContentEditing;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The payment display.
    /// </summary>
    public class PaymentDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        public Guid? CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the payment method key.
        /// </summary>
        public Guid? PaymentMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the payment type field key.
        /// </summary>
        public Guid PaymentTypeFieldKey { get; set; }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentMethodType PaymentMethodType { get; set; } 

        /// <summary>
        /// Gets or sets the payment method name.
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Gets or sets the reference number.
        /// </summary>
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether authorized.
        /// </summary>
        public bool Authorized { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether collected.
        /// </summary>
        public bool Collected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the payment has been voided.
        /// </summary>
        public bool Voided { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether exported.
        /// </summary>
        public bool Exported { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets the applied payments.
        /// </summary>
        public IEnumerable<AppliedPaymentDisplay> AppliedPayments { get; set; }
    }
}