﻿namespace Merchello.FastTrack.Controllers.Payment
{
    using System.Web.Mvc;

    using Merchello.FastTrack.Models.Payment;
    using Merchello.Web.Store.Controllers.Payment;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The FastTrack cash payment controller.
    /// </summary>
    [PluginController("FastTrack")]
    public class CashPaymentController : CashPaymentControllerBase<FastTrackPaymentModel>
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
        protected override ActionResult HandlePaymentSuccess(FastTrackPaymentModel model)
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
    }
}