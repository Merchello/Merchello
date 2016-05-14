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
    [GatewayMethodUi("CashPaymentMethod")]
    public class CashPaymentController : CheckoutPaymentControllerBase<StorePaymentModel>
    {
        /// <summary>
        /// Renders the cash payment form.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        [GatewayMethodUi("CashPaymentMethod")]
        public ActionResult PaymentForm(string view = "")
        {
            var paymentMethod = CheckoutManager.Payment.GetPaymentMethod();
            if (paymentMethod == null) return InvalidCheckoutStagePartial();

            var model = CheckoutPaymentModelFactory.Create(paymentMethod);

            return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
        }
    }
}