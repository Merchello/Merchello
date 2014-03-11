using System;
using System.Linq;
using Merchello.Core.Configuration;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Payment
{
    /// <summary>
    /// Represents a base GatewayPaymentMethod 
    /// </summary>
    public abstract class PaymentGatewayMethodBase : IPaymentGatewayMethod
    {
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IPaymentMethod _paymentMethod;

        protected PaymentGatewayMethodBase(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(paymentMethod, "paymentMethod");

            _gatewayProviderService = gatewayProviderService;
            _paymentMethod = paymentMethod;
        }

        /// <summary>
        /// Does the actual work of creating and authorizing the payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="args">Any arguments required to process the payment. (Maybe a username, password or some Api Key)</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected abstract IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args);

        /// <summary>
        /// Doe the actual work of Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="amount">The amount of the payment to the invoice</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        protected abstract IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args);

        /// <summary>
        /// Does the actual work of capturing a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="payment">The payment to be captured</param>
        /// <param name="amount">The amount of the payment to be captured</param>
        /// <param name="args">Any arguments required to process the payment. (Maybe a username, password or some Api Key)</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected abstract IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args);


        /// <summary>
        /// Does the actual work of refunding the payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        protected abstract IPaymentResult PerformRefundPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args);

        /// <summary>
        /// Processes a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult AuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(invoice, "invoice");
            

            // persist the invoice
            if(!invoice.HasIdentity)
                GatewayProviderService.Save(invoice);

            // collect the payment authorization
            var response = PerformAuthorizePayment(invoice, args);

            if (!response.Result.Success) return response;

            AssertPaymentApplied(response, invoice);
            
            // Check configuration for override on ApproveOrderCreation
            if (!response.ApproveOrderCreation)
                ((PaymentResult)response).ApproveOrderCreation = MerchelloConfiguration.Current.AlwaysApproveOrderCreation;

            // give response
            return response;
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="amount">The amount of the payment to the invoice</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public IPaymentResult AuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(invoice, "invoice");


            // persist the invoice
            if (!invoice.HasIdentity)
                GatewayProviderService.Save(invoice);

            // authorize and capture the payment
            var response = PerformAuthorizeCapturePayment(invoice, amount, args);

            if (!response.Result.Success) return response;

            AssertPaymentApplied(response, invoice);

            // Check configuration for override on ApproveOrderCreation
            if (!response.ApproveOrderCreation)
                ((PaymentResult)response).ApproveOrderCreation = MerchelloConfiguration.Current.AlwaysApproveOrderCreation;

            // give response
            return response;
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="payment">The payment to capture</param>
        /// <param name="amount">The amount of the payment to be captured</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public IPaymentResult CapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(invoice, "invoice");

            // persist the invoice
            if (!invoice.HasIdentity)
                GatewayProviderService.Save(invoice);

            var response = PerformCapturePayment(invoice, payment, amount, args);

            if (!response.Result.Success) return response;

            AssertPaymentApplied(response, invoice);

            // Check configuration for override on ApproveOrderCreation
            if (!response.ApproveOrderCreation)
                ((PaymentResult)response).ApproveOrderCreation = MerchelloConfiguration.Current.AlwaysApproveOrderCreation;

            // give response
            return response;

        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public IPaymentResult RefundPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(invoice, "invoice");
            if(!invoice.HasIdentity) return new PaymentResult(Attempt<IPayment>.Fail(new InvalidOperationException("Cannot refund a payment on an invoice that cannot have payments")), invoice, false);

            var response = PerformRefundPayment(invoice, payment, args);

            if (!response.Result.Success) return response;

            var appliedPayments = payment.AppliedPayments().Where(x => x.TransactionType != AppliedPaymentType.Void);
            foreach (var appliedPayment in appliedPayments)
            {
                appliedPayment.TransactionType = AppliedPaymentType.Void;
                appliedPayment.Amount = 0;
                
                GatewayProviderService.Save(appliedPayment);
            }

            if (invoice.InvoiceStatusKey != Constants.DefaultKeys.InvoiceStatus.Cancelled &&
                invoice.InvoiceStatusKey != Constants.DefaultKeys.InvoiceStatus.Fraud)
            {
                invoice.InvoiceStatusKey =
                    invoice.AppliedPayments().Where(x => x.PaymentKey != payment.Key).Sum(x => x.Amount) > 0
                        ? Constants.DefaultKeys.InvoiceStatus.Paid
                        : Constants.DefaultKeys.InvoiceStatus.Unpaid;

                GatewayProviderService.Save(invoice);

            }
            
            // Force the ApproveOrderCreation to false
            if (response.ApproveOrderCreation) ((PaymentResult)response).ApproveOrderCreation = false;
            
            // give response
            return response;
        }


        private void AssertPaymentApplied(IPaymentResult response, IInvoice invoice)
        {
            // Apply the payment to the invoice if it was not done in the sub class           
            var payment = response.Result.Result;
            if (payment.AppliedPayments(GatewayProviderService).FirstOrDefault(x => x.InvoiceKey == invoice.Key) == null)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, PaymentMethod.Name, payment.Amount);
            }

        }

        /// <summary>
        /// Gets the <see cref="IPaymentMethod"/>
        /// </summary>
        public IPaymentMethod PaymentMethod 
        {
            get { return _paymentMethod; }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderService"/>
        /// </summary>
        protected IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService; }
        }
    }
}