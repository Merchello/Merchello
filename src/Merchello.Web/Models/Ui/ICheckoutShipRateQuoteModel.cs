namespace Merchello.Web.Models.Ui
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Merchello.Core.Gateways.Shipping;

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
        IEnumerable<SelectListItem> ShippingQuotes { get; set; }

        /// <summary>
        /// Gets or sets the collection of quotes from the shipping providers.
        /// </summary>
        IEnumerable<IShipmentRateQuote> ProviderQuotes { get; set; }
    }
}