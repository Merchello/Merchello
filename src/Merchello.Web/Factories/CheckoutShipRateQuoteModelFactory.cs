namespace Merchello.Web.Factories
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Models;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Workflow;

    using Umbraco.Core;

    /// <summary>
    /// A factory responsible for building typed <see cref="ICheckoutShipRateQuoteModel"/> models.
    /// </summary>
    /// <typeparam name="TShipRateQuoteModel">
    /// The type of the <see cref="ICheckoutShipRateQuoteModel"/> to be created
    /// </typeparam>
    public class CheckoutShipRateQuoteModelFactory<TShipRateQuoteModel>
        where TShipRateQuoteModel : class, ICheckoutShipRateQuoteModel, new()
    {
        /// <summary>
        /// The <see cref="IGatewayContext"/>.
        /// </summary>
        private readonly IGatewayContext _gatewayContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutShipRateQuoteModelFactory{TShipRateQuoteModel}"/> class.
        /// </summary>
        public CheckoutShipRateQuoteModelFactory()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutShipRateQuoteModelFactory{TShipRateQuoteModel}"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        public CheckoutShipRateQuoteModelFactory(IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            _gatewayContext = merchelloContext.Gateways;
        }

        /// <summary>
        /// Creates a <see cref="ICheckoutShipRateQuoteModel"/>.
        /// </summary>
        /// <param name="basket">
        /// The <see cref="IBasket"/>.
        /// </param>
        /// <param name="destination">
        /// The destination <see cref="IAddress"/>.
        /// </param>
        /// <param name="tryGetCached">
        /// A value to indicate whether or not to get checked shipping provider quotes.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICheckoutShipRateQuoteModel"/>.
        /// </returns>
        public TShipRateQuoteModel Create(IBasket basket, IAddress destination, bool tryGetCached = true)
        {
            var shipment = basket.PackageBasket(destination).FirstOrDefault();

            var quotes = (shipment != null
                            ? _gatewayContext.Shipping.GetShipRateQuotesForShipment(shipment, tryGetCached).OrderBy(x => x.Rate)
                            : Enumerable.Empty<IShipmentRateQuote>()).ToArray();

            var shipMethodKey = quotes.Any() ? quotes.First().ShipMethod.Key : Guid.Empty;

            var selectItems = quotes.Select(x => new SelectListItem
                {
                    Value = x.ShipMethod.Key.ToString(),
                    Text = string.Format("{0} ({1})", x.ShipMethod.Name, x.Rate.AsFormattedCurrency())
                });

            var model = new TShipRateQuoteModel
                {
                    ShipMethodKey = shipMethodKey,
                    ShippingQuotes = selectItems,
                    ProviderQuotes = quotes 
                };

            return OnCreate(model, basket, destination);
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="ICheckoutShipRateQuoteModel"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutShipRateQuoteModel"/>.
        /// </param>
        /// <param name="basket">
        /// The <see cref="IBasket"/>.
        /// </param>
        /// <param name="address">
        /// The destination <see cref="IAddress"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICheckoutShipRateQuoteModel"/>.
        /// </returns>
        protected TShipRateQuoteModel OnCreate(TShipRateQuoteModel model, IBasket basket, IAddress address)
        {
            return model;
        }
    }
}