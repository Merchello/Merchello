namespace Merchello.FastTrack.Controllers
{
    using System.Web.Mvc;
    using Merchello.FastTrack.Factories;
    using Merchello.FastTrack.Models;
    using Merchello.Web.Controllers;
    using Merchello.Web.Models.Ui;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The controller responsible for rendering and processing addresses in the default checkout process.
    /// </summary>
    [PluginController("FastTrack")]
    public class CheckoutAddressController : CheckoutAddressControllerBase<FastTrackBillingAddressModel, FastTrackCheckoutAddressModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutAddressController"/> class.
        /// </summary>
        public CheckoutAddressController()
            : base(
                  new FastTrackBillingAddressModelFactory(),
                  new FastTrackShippingAddressModelFactory())
        {
        }

        /// <summary>
        /// Overrides the action for a successful billing address save.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected override ActionResult HandleBillingAddressSaveSuccess(FastTrackBillingAddressModel model)
        {
            string redirectUrl;

            if (model.UseForShipping)
            {
                // we use the billing address factory here since we know the model FastTrackBillingAddressModel
                // and only want Merchello's IAddress
                var address = BillingAddressFactory.Create(model);

                CheckoutManager.Customer.SaveShipToAddress(address);

                // In this implementation, we cannot save the customer shipping address to the customer as it may be a different model here
                // However, it is possible but more work would be required to ensure the model mapping

                // set the checkout stage
                model.WorkflowMarker = GetNextCheckoutWorkflowMarker(CheckoutStage.ShippingAddress);

                redirectUrl = model.SuccessUrlShipRateQuote;
            }
            else
            {
                redirectUrl = model.SuccessRedirectUrl;
            }

            // TODO handle AJAX request

            return !redirectUrl.IsNullOrWhiteSpace() ? 
                this.Redirect(redirectUrl) : 
                base.HandleBillingAddressSaveSuccess(model);
        }

        /// <summary>
        /// Overrides the action for a successful shipping address save.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected override ActionResult HandleShippingAddressSaveSuccess(FastTrackCheckoutAddressModel model)
        {
            return !model.SuccessRedirectUrl.IsNullOrWhiteSpace() ?
                Redirect(model.SuccessRedirectUrl) :
                base.HandleShippingAddressSaveSuccess(model);
        }
    }
}