namespace Merchello.Providers.Payment.PayPal.Controllers
{
    using System;
    using System.Net.Http;
    using System.Web.Mvc;

    using Merchello.Core.Logging;
    using Merchello.Core.Services;
    using Merchello.Web.Mvc;

    /// <summary>
    /// The pay pal surface controller base.
    /// </summary>
    public abstract class PayPalSurfaceControllerBase : MerchelloSurfaceController, IPaymentMethodUiController
    {
        /// <summary>
        /// Gets the <see cref="IInvoiceService"/>.
        /// </summary>
        protected IInvoiceService InvoiceService
        {
            get
            {
                return MerchelloServices.InvoiceService;
            }
        }

        /// <summary>
        /// Gets the <see cref="IPaymentService"/>.
        /// </summary>
        protected IPaymentService PaymentService
        {
            get
            {
                return MerchelloServices.PaymentService;
            }
        }

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
        public abstract ActionResult Success(Guid invoiceKey, Guid paymentKey, string token, string payerId);

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
        public abstract ActionResult Cancel(Guid invoiceKey, Guid paymentKey, string token, string payerId = null);

        /// <summary>
        /// Gets the default extended log data.
        /// </summary>
        /// <returns>
        /// The <see cref="IExtendedLoggerData"/>.
        /// </returns>
        protected IExtendedLoggerData GetExtendedLoggerData()
        {
            var logData = MultiLogger.GetBaseLoggingData();
            logData.AddCategory("Controllers");
            logData.AddCategory("PayPal");

            return logData;
        }
    }
}