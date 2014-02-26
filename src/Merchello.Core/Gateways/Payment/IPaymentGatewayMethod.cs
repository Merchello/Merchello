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
        /// Processes a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult ProcessPayment(IInvoice invoice, ProcessorArgumentCollection args);

        /// <summary>
        /// Gets the <see cref="IPaymentMethod"/>
        /// </summary>
        IPaymentMethod PaymentMethod { get; }

    }
}