namespace Merchello.FastTrack.Controllers.Payment
{
    using System.Web.Mvc;
    using Core.Gateways;
    using Models.Payment;
    using Umbraco.Core;
    using Web.Controllers;
    using System;
    using Core.Gateways.Payment;
    using Providers.Payment.PurchaseOrder;
    using Umbraco.Web.Mvc;
    using Web.Models.Ui;

    [PluginController("FastTrack")]
    [GatewayMethodUi("PurchaseOrder.PurchaseOrder")]
    public class PurchaseOrderPaymentController : CheckoutPaymentControllerBase<PurchaseOrderPaymentModel>
    {
        /// <summary>
        /// Handles the redirection for the receipt.
        /// </summary>
        /// <param name="model">
        /// The <see cref="FastTrackPaymentModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected override ActionResult HandlePaymentSuccess(PurchaseOrderPaymentModel model)
        {
            // Set the invoice key in the customer context (cookie)
            if (model.ViewData.Success)
            {
                CustomerContext.SetValue("invoiceKey", model.ViewData.InvoiceKey.ToString());
            }

            return model.ViewData.Success && !model.SuccessRedirectUrl.IsNullOrWhiteSpace() ?
                Redirect(model.SuccessRedirectUrl) :
                base.HandlePaymentSuccess(model);
        }

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
        public virtual ActionResult Process(PurchaseOrderPaymentModel model)
        {
            try
            {
                var paymentMethod = this.CheckoutManager.Payment.GetPaymentMethod();

                // Create the processor argument collection, where we'll pass in the purchase order
                var args = new ProcessorArgumentCollection
                                    {
                                        {PurchaseOrderConstants.PoStringKey, model.PurchaseOrderNumber}
                                    };

                // For PO payments we can only perform an authorize
                var attempt = this.CheckoutManager.Payment.AuthorizePayment(paymentMethod.Key, args);

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

            var model = this.CheckoutPaymentModelFactory.Create(CurrentCustomer, paymentMethod);

            return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
        }
    }
}
