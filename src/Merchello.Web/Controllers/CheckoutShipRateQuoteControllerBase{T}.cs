namespace Merchello.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Models;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;

    using Umbraco.Core;

    /// <summary>
    /// The checkout ship rate quote controller.
    /// </summary>
    /// <typeparam name="TShipRateQuote">
    /// The type of <see cref="ICheckoutShipRateQuoteModel"/>
    /// </typeparam>
    public abstract class CheckoutShipRateQuoteControllerBase<TShipRateQuote> : CheckoutControllerBase
        where TShipRateQuote : class, ICheckoutShipRateQuoteModel, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutShipRateQuoteControllerBase{TShipRateQuote}"/> class.
        /// </summary>
        protected CheckoutShipRateQuoteControllerBase()
            : this(new CheckoutShipRateQuoteModelFactory<TShipRateQuote>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutShipRateQuoteControllerBase{TShipRateQuote}"/> class.
        /// </summary>
        /// <param name="checkoutShipRateQuotFactory">
        /// The <see cref="CheckoutShipRateQuoteControllerBase{TShipRateQuote}"/>.
        /// </param>
        protected CheckoutShipRateQuoteControllerBase(
            CheckoutShipRateQuoteModelFactory<TShipRateQuote> checkoutShipRateQuotFactory)
            : this(checkoutShipRateQuotFactory, new CheckoutContextSettingsFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutShipRateQuoteControllerBase{TShipRateQuote}"/> class.
        /// </summary>
        /// <param name="checkoutShipRateQuoteFactory">
        /// The <see cref="CheckoutShipRateQuoteModelFactory{TShipRateQuoteModel}"/>.
        /// </param>
        /// <param name="contextSettingsFactory">
        /// The <see cref="CheckoutContextSettingsFactory"/>.
        /// </param>
        protected CheckoutShipRateQuoteControllerBase(
            CheckoutShipRateQuoteModelFactory<TShipRateQuote> checkoutShipRateQuoteFactory,
            CheckoutContextSettingsFactory contextSettingsFactory)
            : base(contextSettingsFactory)
        {
            Mandate.ParameterNotNull(checkoutShipRateQuoteFactory, "checkoutShipRateQuoteFactory");
            this.CheckoutShipRateQuoteFactory = checkoutShipRateQuoteFactory;
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="CheckoutShipRateQuoteModelFactory{TShipRateQuote}"/>.
        /// </summary>
        protected CheckoutShipRateQuoteModelFactory<TShipRateQuote> CheckoutShipRateQuoteFactory { get; private set; }

        /// <summary>
        /// Saves the shipment rate quote.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutShipRateQuoteModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public virtual ActionResult SaveShipRateQuote(TShipRateQuote model)
        {
            try
            {
                var shippingAddress = CheckoutManager.Customer.GetShipToAddress();
                if (shippingAddress == null) return CurrentUmbracoPage();

                if (!ModelState.IsValid) return CurrentUmbracoPage();

                CheckoutManager.Shipping.ClearShipmentRateQuotes();

                var quoteModel = CheckoutShipRateQuoteFactory.Create(Basket, shippingAddress);

                // merge the models for return override
                model.ShippingQuotes = quoteModel.ShippingQuotes;
                model.ProviderQuotes = quoteModel.ProviderQuotes;

                var accepted = quoteModel.ProviderQuotes.FirstOrDefault(x => x.ShipMethod.Key == model.ShipMethodKey);
                if (accepted == null) return CurrentUmbracoPage();

                CheckoutManager.Shipping.SaveShipmentRateQuote(accepted);

                return HandleShipRateQuoteSaveSuccess(model);
            }
            catch (Exception ex)
            {
                return this.HandleShipRateQuoteSaveException(model, ex);
            }
        }

        #region ChildActions

        /// <summary>
        /// Responsible for rendering the shipment rate quote form.
        /// </summary>
        /// <param name="view">
        /// The optional view name.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public ActionResult ShipRateQuoteForm(string view = "")
        {
            var shippingAddress = CheckoutManager.Customer.GetShipToAddress();
            if (shippingAddress == null) return PartialView("InvalidCheckoutStage");

            var model = CheckoutShipRateQuoteFactory.Create(Basket, shippingAddress);

            return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);

        }

        #endregion

        /// <summary>
        /// Allows for overriding the action of a successful shipping rate quote save.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutShipRateQuoteModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult HandleShipRateQuoteSaveSuccess(TShipRateQuote model)
        {
            return this.RedirectToCurrentUmbracoPage();
        }

        /// <summary>
        /// Allows for overriding the action of a exception durring shipping rate quote save operation.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutShipRateQuoteModel"/>.
        /// </param>
        /// <param name="ex">
        /// The <see cref="Exception"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// The <see cref="Exception"/> to be handled.
        /// </exception>
        protected virtual ActionResult HandleShipRateQuoteSaveException(TShipRateQuote model, Exception ex)
        {
            throw ex;
        }
    }
}
