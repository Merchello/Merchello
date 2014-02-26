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

        /// <summary>
        /// Does the actual work of creating and processing the payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="args">Any arguments required to process the payment. (Maybe a username, password or some Api Key)</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected override IPaymentResult PerformProcessPayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            var payment = GatewayProviderService.CreatePaymentWithKey(PaymentMethodType.Cash, invoice.Total, PaymentMethod.Key);
            
            payment.PaymentMethodName = PaymentMethod.Name + " " + PaymentMethod.PaymentCode;
            payment.ReferenceNumber = PaymentMethod.PaymentCode + "-" + invoice.PrefixedInvoiceNumber();
            payment.Collected = false;
            payment.Authorized = true;

            GatewayProviderService.Save(payment);

            // In this case, we want to do our own Apply Payment operation as the amount has not been collected -
            // so we create an applied payment with a 0 amount.  Once the payment has been "collected", another Applied Payment record will
            // be created showing the full amount and the invoice status will be set to Paid.
            var appliedPayment = GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, string.Format("To show promise of a {0} payment", PaymentMethod.Name), 0);

            // If this were using a service we might want to store some of the transaction data in the ExtendedData for record
            //payment.ExtendData


            return new PaymentResult(Attempt.Succeed(payment), false);
        }
    }
}