namespace Merchello.Providers.Payment.PayPal.Controllers
{
    using System;
    using System.Web.Mvc;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// Defines base a PayPal <see cref="SurfaceController"/>.
    /// </summary>
    public interface IPayPalSurfaceController
    {
        /// <summary>
        /// Handles a successful return from PayPal
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="payerId">
        /// The payer id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        ActionResult Success(Guid invoiceKey, Guid paymentKey, string token, string payerId);

        /// <summary>
        /// Handles a cancellation response from PayPal
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="payerId">
        /// The payer id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        ActionResult Cancel(Guid invoiceKey, Guid paymentKey, string token, string payerId = null);
    }
}