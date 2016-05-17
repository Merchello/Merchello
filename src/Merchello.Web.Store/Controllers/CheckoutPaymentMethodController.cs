namespace Merchello.Web.Store.Controllers
{
    using Merchello.Web.Controllers;
    using Merchello.Web.Store.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// A controller responsible for handling payment method selection operations.
    /// </summary>
    [PluginController("Merchello")]
    public class CheckoutPaymentMethodController : CheckoutPaymentMethodControllerBase<StorePaymentMethodModel>
    {
    }
}
