namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;
    using Merchello.Core.Checkout;
    using Merchello.Bazaar.Models;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;
    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// Payment method controller for the Cash Payment Method.
    /// </summary>
    [PluginController("Bazaar")]
    [GatewayMethodUi("CashPaymentMethod")]
    public class CashPaymentMethodController : BazaarPaymentMethodFormControllerBase
    {
        /// <summary>
        /// Responsible for rendering the Cash Payment Method Form.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public override ActionResult RenderForm(CheckoutConfirmationForm model)
        {
            return this.PartialView(PathHelper.GetThemePartialViewPath(model.ThemeName, "CashPaymentMethodForm"), model);
        }

        /// <summary>
        /// Responsible for actually processing the payment with the PaymentProvider
        /// </summary>
        /// <param name="preparation">
        /// The preparation.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        protected override IPaymentResult PerformProcessPayment(ICheckoutManagerBase preparation, IPaymentMethod paymentMethod)
        {
            return preparation.Payment.AuthorizePayment(paymentMethod.Key);
        }
    }
}