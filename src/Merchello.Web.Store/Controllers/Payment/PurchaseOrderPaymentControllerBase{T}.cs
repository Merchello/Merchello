namespace Merchello.Web.Store.Controllers.Payment
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Providers.Payment.PurchaseOrder;
    using Merchello.Web.Controllers;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Models;

    using Umbraco.Core;

    using Constants = Merchello.Providers.Constants;

    /// <summary>
    /// Represents a PurchaseOrder Payment Controller.
    /// </summary>
    /// <typeparam name="TPaymentModel">
    /// The type of the payment model
    /// </typeparam>
    [GatewayMethodUi("PurchaseOrder.PurchaseOrder")]
    public abstract class PurchaseOrderPaymentControllerBase<TPaymentModel> : CheckoutPaymentControllerBase<TPaymentModel>
        where TPaymentModel : class, IPurchaseOrderPaymentModel, new()
    {
        /// <summary>
        /// Processes the PO payment.
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

                // Create the processor argument collection, where we'll pass in the purchase order
                var args = new ProcessorArgumentCollection
                                    {
                                        { Constants.PurchaseOrder.PoStringKey, model.PurchaseOrderNumber }
                                    };

                // For PO payments we can only perform an authorize
                var attempt = this.CheckoutManager.Payment.AuthorizePayment(paymentMethod.Key, args);

                var resultModel = this.CheckoutPaymentModelFactory.Create(this.CurrentCustomer, paymentMethod, attempt);

                // merge the models so we can be assured that any hidden values are passed on
                model.ViewData = resultModel.ViewData;

                // Send the notification
                this.HandleNotificiation(model, attempt);

                return this.HandlePaymentSuccess(model);
            }
            catch (Exception ex)
            {
                return this.HandlePaymentException(model, ex);
            }
        }

        /// <summary>
        /// Renders the Purchase Order payment form.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        [GatewayMethodUi("PurchaseOrder.PurchaseOrder")]
        public override ActionResult PaymentForm(string view = "")
        {
            var paymentMethod = this.CheckoutManager.Payment.GetPaymentMethod();
            if (paymentMethod == null) return this.InvalidCheckoutStagePartial();

            var model = this.CheckoutPaymentModelFactory.Create(this.CurrentCustomer, paymentMethod);

            return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
        }
    }
}
