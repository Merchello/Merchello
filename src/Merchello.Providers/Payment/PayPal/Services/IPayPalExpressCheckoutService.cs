namespace Merchello.Providers.Payment.PayPal.Services
{
    using Merchello.Core.Models;

    using Merchello.Providers.Payment.PayPal.Models;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines a PayPalExpressCheckoutService.
    /// </summary>
    public interface IPayPalExpressCheckoutService : IService
    {
        /// <summary>
        /// Performs the setup for an express checkout.
        /// </summary>
        /// <param name="invoice">
        /// The <see cref="IInvoice"/>.
        /// </param>
        /// <param name="payment">
        /// The <see cref="IPayment"/>
        /// </param>
        /// <returns>
        /// The <see cref="ExpressCheckoutResponse"/>.
        /// </returns>
        PayPalExpressTransactionRecord SetExpressCheckout(IInvoice invoice, IPayment payment);


        /// <summary>
        /// The capture success.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="isPartialPayment">
        /// The is partial payment.
        /// </param>
        /// <returns>
        /// The <see cref="ExpressCheckoutResponse"/>.
        /// </returns>
        PayPalExpressTransactionRecord Capture(IInvoice invoice, IPayment payment, decimal amount, bool isPartialPayment);
    }
} 