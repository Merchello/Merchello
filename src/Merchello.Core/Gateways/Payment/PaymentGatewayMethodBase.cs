using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Payment
{
    /// <summary>
    /// Represents a base GatewayPaymentMethod 
    /// </summary>
    public abstract class PaymentGatewayMethodBase : IPaymentGatewayMethod
    {
        private readonly IPaymentMethod _paymentMethod;

        protected PaymentGatewayMethodBase(IPaymentMethod paymentMethod)
        {
            Mandate.ParameterNotNull(paymentMethod, "paymentMethod");

            _paymentMethod = paymentMethod;
        }

        /// <summary>
        /// Processes a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentGatewayResponse"/></returns>
        public abstract IPaymentGatewayResponse ProcessPayment(IInvoice invoice, ProcessorArgumentCollection args);
        
        /// <summary>
        /// Gets the <see cref="IPaymentMethod"/>
        /// </summary>
        public IPaymentMethod PaymentMethod 
        {
            get { return _paymentMethod; }
        }
        
    }
}