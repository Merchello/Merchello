namespace Merchello.FastTrack.Controllers.Payment
{
    using System.Web.Mvc;

    using Merchello.FastTrack.Models.Payment;
    using Merchello.Web.Store.Controllers.Payment;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// Represents a Purchase Order Payment Controller.
    /// </summary>
    [PluginController("FastTrack")]
    public class PurchaseOrderPaymentController : PurchaseOrderPaymentControllerBase<PurchaseOrderPaymentModel>
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
                this.CustomerContext.SetValue("invoiceKey", model.ViewData.InvoiceKey.ToString());
            }

            return model.ViewData.Success && !model.SuccessRedirectUrl.IsNullOrWhiteSpace() ?
                this.Redirect(model.SuccessRedirectUrl) :
                base.HandlePaymentSuccess(model);
        }
    }
}