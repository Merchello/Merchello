namespace Merchello.Web.Store.Controllers
{
    using Merchello.Web.Controllers;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default checkout summary controller.
    /// </summary>
    [PluginController("Merchello")]
    public class CheckoutSummaryController : CheckoutSummaryControllerBase<CheckoutSummaryModel, CheckoutAddressModel, CheckoutAddressModel, BasketItemModel>
    {

    }
}