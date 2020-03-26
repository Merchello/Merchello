using Merchello.Providers.Payment.Braintree.Controllers;
using Merchello.Providers.Payment.Braintree.Models;

namespace Merchello.FastTrack.Controllers.Payment
{
    using System.Web.Mvc;

    using Merchello.Core.Gateways;
    using Merchello.Web.Models.Ui.Async;
    using Merchello.Web.Store.Controllers.Payment;
    using Merchello.Web.Store.Models;
    using Merchello.Web.Store.Models.Async;

    using Umbraco.Core;

    /// <summary>
    /// A base class for FastTrack Braintree Payment Controllers.
    /// </summary>
    /// <typeparam name="TPaymentModel">
    /// The type of the <see cref="BraintreePaymentModel"/>
    /// </typeparam>
    public abstract class FastTrackBraintreeControllerBase<TPaymentModel> : BraintreePaymentControllerBase<TPaymentModel>
        where TPaymentModel : BraintreePaymentModel, new()
    {

        /// <summary>
        /// Responsible for rendering the BrainTree PayPal payment form.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        [GatewayMethodUi("BrainTree.PayPal.OneTime")]
        [GatewayMethodUi("BrainTree.StandardTransaction")]
        public override ActionResult PaymentForm(string view = "")
        {
            var paymentMethod = this.CheckoutManager.Payment.GetPaymentMethod();
            if (paymentMethod == null) return this.InvalidCheckoutStagePartial();

            var model = this.CheckoutPaymentModelFactory.Create(CurrentCustomer, paymentMethod);

            return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
        }

        /// <summary>
        /// Overrides the payment success.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected override ActionResult HandlePaymentSuccess(TPaymentModel model)
        {
            // Set the invoice key in the customer context (cookie)
            if (model.ViewData.Success)
            {
                CustomerContext.SetValue("invoiceKey", model.ViewData.InvoiceKey.ToString());
            }

            if (Request.IsAjaxRequest())
            {
                var json = Json(GetAsyncResponse(model));

                return json;
            }

            return base.HandlePaymentSuccess(model);
        }

        /// <summary>
        /// Gets the <see cref="PaymentResultAsyncResponse"/> for the model.
        /// </summary>
        /// <param name="model">
        /// The <see cref="BraintreePaymentModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentResultAsyncResponse"/>.
        /// </returns>
        protected virtual PaymentResultAsyncResponse GetAsyncResponse(BraintreePaymentModel model)
        {
            var resp = new PaymentResultAsyncResponse
            {
                Success = model.ViewData.Success,
                InvoiceKey = model.ViewData.InvoiceKey,
                PaymentKey = model.ViewData.PaymentKey,
                ItemCount = GetBasketItemCountForDisplay(),
                PaymentMethodName = model.PaymentMethodName
            };

            foreach (var msg in model.ViewData.Messages) resp.Messages.Add(msg);

            return resp;
        }
    }
}