namespace Merchello.FastTrack.Controllers
{
    using Merchello.FastTrack.Models;
    using Merchello.Web.Controllers;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default checkout summary controller.
    /// </summary>
    [PluginController("FastTrack")]
    public class CheckoutSummaryController : CheckoutSummaryControllerBase<FastTrackCheckoutSummary, FastTrackBillingAddressModel, CheckoutAddressModel, BasketItemModel>
    {
    }
}