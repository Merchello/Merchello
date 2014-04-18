using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Payment
{
    /// <summary>
    /// Defines a GatewayPaymentMethod
    /// </summary>
    /// <remarks>
    /// 
    /// There will be a breaking change when we expose customers as we will want to require
    /// a couple of additional methods - eg.
    /// 
    /// ProcessPayment(ICustomer customer, decimal amount) - which would add a customer credit (an unapplied payment)
    /// 
    /// </remarks>
    public interface IPaymentGatewayMethod : IGatewayMethod
    {

        /// <summary>
        /// Authorizes a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args);

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="amount">The amount of the payment to the invoice</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args);

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="payment">The</param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult CapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args);


        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="amount">The amount of the payment to be refunded</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult RefundPayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args);

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="invoice">The invoice assoicated with the payment to be voided</param>
        /// <param name="payment">The payment to be voided</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult VoidPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args);

        /// <summary>
        /// Gets the <see cref="IPaymentMethod"/>
        /// </summary>
        IPaymentMethod PaymentMethod { get; }

    }
}