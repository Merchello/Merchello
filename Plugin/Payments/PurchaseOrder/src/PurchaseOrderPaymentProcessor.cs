using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Plugin.Payments.PurchaseOrder.Models;
using Umbraco.Core;
using Constants = Merchello.Plugin.Payments.PurchaseOrder.Constants;

namespace Merchello.Plugin.Payments.PurchaseOrder
{
    /// <summary>
    /// The Authorize.Net payment processor
    /// </summary>
    public class PurchaseOrderPaymentProcessor
    {
        private readonly PurchaseOrderProcessorSettings _settings;

        public PurchaseOrderPaymentProcessor(PurchaseOrderProcessorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Processes the Authorize and AuthorizeAndCapture transactions
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to be paid</param>
        /// <param name="payment">The <see cref="IPayment"/> record</param>
        /// <param name="transactionMode">Authorize or AuthorizeAndCapture</param>
        /// <param name="amount">The money amount to be processed</param>
        /// <param name="purchaseOrder">The <see cref="PurchaseOrderFormData"></see></param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public IPaymentResult ProcessPayment(IInvoice invoice, IPayment payment, decimal amount, PurchaseOrderFormData purchaseOrder)
        {
            if (string.IsNullOrEmpty(purchaseOrder.PurchaseOrderNumber))
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Error Purchase Order Number is empty"))), invoice, false);
            }

            invoice.PoNumber = purchaseOrder.PurchaseOrderNumber;         
            payment.Authorized = true;
            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
        }

        /// <summary>
        /// Captures a previously authorized payment
        /// </summary>
        /// <param name="invoice">The invoice associated with the <see cref="IPayment"/></param>
        /// <param name="payment">The <see cref="IPayment"/> to capture</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public IPaymentResult PriorAuthorizeCapturePayment(IInvoice invoice, IPayment payment)
        {
            if (!payment.Authorized) return new PaymentResult(Attempt<IPayment>.Fail(payment, new InvalidOperationException("Payment is not Authorized")), invoice, false);
               
            payment.Collected = true;
            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
        }

        /// <summary>
        /// Refunds a payment amount
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> associated with the payment</param>
        /// <param name="payment">The <see cref="IPayment"/> to be refunded</param>
        /// <param name="amount">The amount of the <see cref="IPayment"/> to be refunded</param>
        /// <returns></returns>
        public IPaymentResult RefundPayment(IInvoice invoice, IPayment payment, decimal amount)
        {                          
            if (!payment.Authorized) return new PaymentResult(Attempt<IPayment>.Fail(payment, new InvalidOperationException("Payment is not Authorized")), invoice, false);
           
            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
        }

        public IPaymentResult VoidPayment(IInvoice invoice, IPayment payment)
        {
            if (!payment.Authorized) return new PaymentResult(Attempt<IPayment>.Fail(payment, new InvalidOperationException("Payment is not Authorized")), invoice, false);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
        }
    }
}