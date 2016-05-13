namespace Merchello.Web.Models.Ui
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Localization;

    /// <summary>
    /// A model for quoting shipments.
    /// </summary>
    public interface ICheckoutShipRateQuoteModel : ICheckoutModel
    {
        /// <summary>
        /// Gets or sets the ship method key.
        /// </summary>
        Guid ShipMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the shipping quotes.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelShipRateQuote")]
        IEnumerable<SelectListItem> ShippingQuotes { get; set; }

        /// <summary>
        /// Gets or sets the collection of quotes from the shipping providers.
        /// </summary>
        IEnumerable<IShipmentRateQuote> ProviderQuotes { get; set; }
    }
}