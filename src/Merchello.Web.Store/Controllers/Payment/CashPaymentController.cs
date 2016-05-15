namespace Merchello.Web.Store.Controllers.Payment
{
    using System.Web.Mvc;

    using Merchello.Core.Gateways;
    using Merchello.Web.Controllers;
    using Merchello.Web.Store.Models;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// A controller responsible for rendering and accepting cash payments.
    /// </summary>
    [PluginController("Merchello")]
    public class CashPaymentController : CashPaymentControllerBase<StorePaymentModel>
    {
    }
}