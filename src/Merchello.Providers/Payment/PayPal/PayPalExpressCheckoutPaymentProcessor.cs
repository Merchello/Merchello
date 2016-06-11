namespace Merchello.Providers.Payment.PayPal
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Providers.Payment.PayPal.Models;
    using Merchello.Providers.Payment.PayPal.Services;

    using Umbraco.Core;

    /// <summary>
	/// The PayPal payment processor
	/// </summary>
	internal class PayPalExpressCheckoutPaymentProcessor
	{
        /// <summary>
        /// The <see cref="IPayPalApiPaymentService"/>.
        /// </summary>
        private readonly PayPalExpressCheckoutService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalExpressCheckoutPaymentProcessor"/> class.
        /// </summary>
        /// <param name="service">
        /// The <see cref="IPayPalApiPaymentService"/>.
        /// </param>
        public PayPalExpressCheckoutPaymentProcessor(IPayPalApiService service)
        {
            Mandate.ParameterNotNull(service, "service");
            this._service = (PayPalExpressCheckoutService)service.ExpressCheckout;
        }

        /// <summary>
        /// Verifies the authorization of a success return.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <returns>
        /// The <see cref="PayPalExpressTransactionRecord"/>.
        /// </returns>
        public PayPalExpressTransactionRecord VerifySuccessAuthorziation(IInvoice invoice, IPayment payment)
        {
            // We need to process several transactions in a row to get all the data we need to record the
            // transaction with enough information to do refunds / partial refunds
            var record = payment.GetPayPalTransactionRecord();
            if (record == null || record.SetExpressCheckout == null || record.Data.Token.IsNullOrWhiteSpace())
            {
                throw new NullReferenceException("PayPal ExPress Checkout must be setup");
            }

            record = _service.GetExpressCheckoutDetails(payment, record.Data.Token, record);
            if (!record.Success) return record;

            record = Process(payment, _service.DoExpressCheckoutPayment(invoice, payment, record.Data.Token, record.Data.PayerId, record));
            if (!record.Success) return record;

            record = Process(payment, _service.Authorize(invoice, payment, record.Data.Token, record.Data.PayerId, record));
            return record;
        }

        /// <summary>
        /// Processes the payment.
        /// </summary>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <returns>
        /// The <see cref="PayPalExpressTransactionRecord"/>.
        /// </returns>
        private PayPalExpressTransactionRecord Process(IPayment payment, PayPalExpressTransactionRecord record)
        {
            payment.SavePayPalTransactionRecord(record);
            return record;
        }
	}
}
