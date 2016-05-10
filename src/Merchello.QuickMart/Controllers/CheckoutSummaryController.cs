namespace Merchello.QuickMart.Controllers
{
    using Merchello.QuickMart.Models;
    using Merchello.Web.Controllers;
    using Merchello.Web.Store.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default checkout summary controller.
    /// </summary>
    [PluginController("QuickMart")]
    public class CheckoutSummaryController : CheckoutSummaryControllerBase<QuickMartCheckoutSummary, CheckoutBillingAddressModel, CheckoutAddressModel, BasketItemModel>
    {
    }
}