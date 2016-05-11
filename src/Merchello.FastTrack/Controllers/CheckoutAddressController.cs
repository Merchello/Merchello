namespace Merchello.FastTrack.Controllers
{
    using Merchello.FastTrack.Factories;
    using Merchello.FastTrack.Models;
    using Merchello.QuickMart.Factories;
    using Merchello.Web.Controllers;
    using Merchello.Web.Factories;
    using Merchello.Web.Store.Models;

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
    }
}