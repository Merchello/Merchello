namespace Merchello.Bazaar.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Models;

    /// <summary>
    /// The checkout confirmation form.
    /// </summary>
    public class CheckoutConfirmationForm
    {
        /// <summary>
        /// Gets or sets the theme name.
        /// </summary>
        public string ThemeName { get; set; }

        /// <summary>
        /// Gets or sets the sale summary.
        /// </summary>
        public SalePreparationSummary SaleSummary { get; set; }

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
        /// Gets or sets the receipt page id.
        /// </summary>
        public int ReceiptPageId { get; set; }
    }
}