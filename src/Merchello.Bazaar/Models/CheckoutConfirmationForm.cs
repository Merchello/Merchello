namespace Merchello.Bazaar.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    /// <summary>
    /// The checkout confirmation form.
    /// </summary>
    public partial class CheckoutConfirmationForm
    {
        /// <summary>
        /// Gets or sets the theme name.
        /// </summary>
        public string ThemeName { get; set; }

        /// <summary>
        /// Gets or sets the customer token.
        /// </summary>
        public string CustomerToken { get; set; }

        /// <summary>
        /// Gets or sets the sale summary.
        /// </summary>
        public InvoiceSummary InvoiceSummary { get; set; }

        /// <summary>
        /// Gets or sets the ship method key.
        /// </summary>
        public Guid ShipMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the payment method key.
        /// </summary>
        public Guid PaymentMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the processor args.
        /// </summary>
        public Dictionary<string, string> ProcessorArgs { get; set; }  

        /// <summary>
        /// Gets or sets the shipping quotes.
        /// </summary>
        [Display(Name = "Shipping")]
        public IEnumerable<SelectListItem> ShippingQuotes { get; set; }

        /// <summary>
        /// Gets or sets the payment methods.
        /// </summary>
        [Display(Name = "Method of payment")]
        public IEnumerable<SelectListItem> PaymentMethods { get; set; }

        /// <summary>
        /// Gets or sets the payment method forms.
        /// </summary>
        public IEnumerable<PaymentMethodUiInfo> PaymentMethodUiInfo { get; set; } 

        /// <summary>
        /// Gets or sets the receipt page id.
        /// </summary>
        public int ReceiptPageId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to resolve payment forms.
        /// </summary>
        public bool ResolvePaymentForms { get; set; }
    }
}