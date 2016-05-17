namespace Merchello.Web.Store.Controllers.Payment
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core.Gateways;
    using Merchello.Web.Controllers;
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
                var paymentMethod = this.CheckoutManager.Payment.GetPaymentMethod();

                // For cash payments we can only perform an authorize
                var attempt = this.CheckoutManager.Payment.AuthorizePayment(paymentMethod.Key);

                var resultModel = this.CheckoutPaymentModelFactory.Create(CurrentCustomer, paymentMethod, attempt);

                // merge the models so we can be assured that any hidden values are passed on
                model.ViewData = resultModel.ViewData;

                // Send the notification
                HandleNotificiation(model, attempt);

                return this.HandlePaymentSuccess(model);
            }
            catch (Exception ex)
            {
                return this.HandlePaymentException(model, ex);
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
        public override ActionResult PaymentForm(string view = "")
        {
            var paymentMethod = this.CheckoutManager.Payment.GetPaymentMethod();
            if (paymentMethod == null) return this.InvalidCheckoutStagePartial();

            var model = this.CheckoutPaymentModelFactory.Create(CurrentCustomer, paymentMethod);

            return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
        }
    }
}