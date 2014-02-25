using System;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Payment.Cash
{
    public class CashPaymentMethod : PaymentGatewayMethodBase, ICashPaymentMethod
    {
        public CashPaymentMethod(IPaymentMethod paymentMethod) 
            : base(paymentMethod)
        {
        }

        /// <summary>
        /// Processes a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentGatewayResponse"/></returns>
        public override IPaymentGatewayResponse ProcessPayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(invoice, "invoice");

            throw new NotImplementedException();
        }
    }
}