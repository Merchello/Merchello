namespace Merchello.FastTrack.Controllers
{
    using Merchello.FastTrack.Factories;
    using Merchello.FastTrack.Models;
    using Merchello.Web.Controllers;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default checkout summary controller.
    /// </summary>
    [PluginController("FastTrack")]
    public class CheckoutSummaryController : CheckoutSummaryControllerBase<FastTrackCheckoutSummary, FastTrackBillingAddressModel, CheckoutAddressModel, BasketItemModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutSummaryController"/> class.
        /// </summary>
        public CheckoutSummaryController()
            : base(
                  new FastTrackCheckoutSummaryModelFactory(),
                  new CheckoutContextSettingsFactory())
        {
        }
    }
}