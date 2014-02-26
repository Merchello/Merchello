using System;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Payment.Cash
{
    /// <summary>
    /// Represents a CashPaymentMethod
    /// </summary>
    public class CashPaymentMethod : PaymentGatewayMethodBase, ICashPaymentMethod
    {
        public CashPaymentMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod) 
            : base(gatewayProviderService, paymentMethod)
        { }

        protected override IPaymentResult PerformProcessPayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            var payment = GatewayProviderService.CreatePaymentWithKey(PaymentMethodType.Cash, invoice.Total, PaymentMethod.Key);

            payment.PaymentMethodName = PaymentMethod.Name + " " + PaymentMethod.PaymentCode;
            payment.ReferenceNumber = PaymentMethod.PaymentCode + "-" + invoice.PrefixedInvoiceNumber();

            // If this were using a service we might want to store some of the transaction data in the ExtendedData for record
            //payment.ExtendData


            return new PaymentResult(Attempt.Succeed(payment), false);
        }
    }
}