namespace Merchello.QuickMart.Controllers
{
    using Merchello.QuickMart.Models;
    using Merchello.Web.Controllers;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default checkout summary controller.
    /// </summary>
    [PluginController("QuickMart")]
    public class QuickMartCheckoutSummaryController : CheckoutSummaryControllerBase<CheckoutSummaryModel, CheckoutBillingAddressModel, CheckoutAddressModel, BasketItemModel>
    {
    }
}