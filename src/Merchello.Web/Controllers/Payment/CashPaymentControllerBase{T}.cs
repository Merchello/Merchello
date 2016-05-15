namespace Merchello.Web.Controllers.Payment
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core.Gateways;
    using Merchello.Web.Models.Ui;

    using Umbraco.Core;

    /// <summary>
    /// A base class for cash payment controllers.
    /// </summary>
    /// <typeparam name="TPaymentModel">
    /// The type of <see cref="ICheckoutPaymentModel"/>
    /// </typeparam>
    [GatewayMethodUi("CashPaymentMethod")]
    public abstract class CashPaymentControllerBase<TPaymentModel> : CheckoutPaymentControllerBase<TPaymentModel>
        where TPaymentModel : class, ICheckoutPaymentModel, new()
    {
        /// <summary>
        /// Processes the cash payment.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutPaymentModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public virtual ActionResult Process(TPaymentModel model)
        {
            try
            {
                var paymentMethod = CheckoutManager.Payment.GetPaymentMethod();

                // For cash payments we can only perform an authorize
                var attempt = CheckoutManager.Payment.AuthorizePayment(paymentMethod.Key);

                var resultModel = CheckoutPaymentModelFactory.Create(paymentMethod, attempt);

                // merge the models so we can be assured that any hidden values are passed on
                model.ViewData = resultModel.ViewData;

                return HandlePaymentSuccess(model);
            }
            catch (Exception ex)
            {
                return HandlePaymentException(model, ex);
            }
        }

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