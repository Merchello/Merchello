namespace Merchello.Web.Models.Ui
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Localization;

    /// <summary>
    /// A model for quoting checkout shipment rate.
    /// </summary>
    public class CheckoutShipRateQuoteModel : ICheckoutShipRateQuoteModel
    {
        /// <summary>
        /// Gets or sets the ship method key.
        /// </summary>
        /// <remarks>
        /// The key used to save the selected quote in the post back
        /// </remarks>
        public Guid ShipMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the shipping quotes.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelShipRateQuote")]
        public IEnumerable<SelectListItem> ShippingQuotes { get; set; }

        /// <summary>
        /// Gets or sets the quotes that obtained from the provider.
        /// </summary>
        public IEnumerable<IShipmentRateQuote> ProviderQuotes { get; set; }

        /// <summary>
        /// Gets or sets the workflow marker.
        /// </summary>
        /// <remarks>
        /// This is used to assist in tracking the checkout - generally in single page checkouts
        /// </remarks>
        public ICheckoutWorkflowMarker WorkflowMarker { get; set; }
    }
}