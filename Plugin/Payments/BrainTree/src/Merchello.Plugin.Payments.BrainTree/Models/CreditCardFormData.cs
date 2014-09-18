namespace Merchello.Plugin.Payments.Braintree.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The credit card form data.
    /// </summary>
    public class CreditCardFormData
    {
        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        public Guid CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the payment_method_nonce.  This is the token used in the API
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        public string payment_method_nonce { get; set; }

        /// <summary>
        /// Gets or sets the customer's IP Address
        /// </summary>
        public string CustomerIp { get; set; }
    }
}