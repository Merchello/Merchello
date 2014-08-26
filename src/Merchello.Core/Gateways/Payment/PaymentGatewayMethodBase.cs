namespace Merchello.Core.Gateways.Payment
{
    using System;
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// Represents a base GatewayPaymentMethod 
    /// </summary>
    public abstract class PaymentGatewayMethodBase : IPaymentGatewayMethod
    {
        /// <summary>
        /// The _gateway provider service.
        /// </summary>
        private readonly IGatewayProviderService _gatewayProviderService;

        /// <summary>
        /// The _payment method.
        /// </summary>
        private readonly IPaymentMethod _paymentMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayMethodBase"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        protected PaymentGatewayMethodBase(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(paymentMethod, "paymentMethod");

            _gatewayProviderService = gatewayProviderService;
            _paymentMethod = paymentMethod;
        }

        #region Events       

        /// <summary>
        /// The authorize attempted event handler. Fires after an authorize attempt.
        /// </summary>
        public static event TypedEventHandler<PaymentGatewayMethodBase, PaymentAttemptEventArgs<IPaymentResult>> AuthorizeAttempted;

        /// <summary>
        /// The capture attempted event handler.  Fires after a capture attempt
        /// </summary>
        public static event TypedEventHandler<PaymentGatewayMethodBase, PaymentAttemptEventArgs<IPaymentResult>> CaptureAttempted;

        /// <summary>
        /// The authorize capture attempted event handler.  Fires after an authorize / capture attempt
        /// </summary>
        public static event TypedEventHandler<PaymentGatewayMethodBase, PaymentAttemptEventArgs<IPaymentResult>> AuthorizeCaptureAttempted;

        /// <summary>
        /// The void attempted event handler. Fires after a void attempt
        /// </summary>
        public static event TypedEventHandler<PaymentGatewayMethodBase, PaymentAttemptEventArgs<IPaymentResult>> VoidAttempted;

        /// <summary>
        /// The refund attempted event handler.  Fires after a refund attempt
        /// </summary>
        public static event TypedEventHandler<PaymentGatewayMethodBase, PaymentAttemptEventArgs<IPaymentResult>> RefundAttempted;

        #endregion

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

        /// <summary>
        /// Processes a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be paid</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult AuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(invoice, "invoice");


            // persist the invoice
            if (!invoice.HasIdentity)
                GatewayProviderService.Save(invoice);

            // collect the payment authorization
            var response = PerformAuthorizePayment(invoice, args);

            AuthorizeAttempted.RaiseEvent(new PaymentAttemptEventArgs<IPaymentResult>(response), this);

            if (!response.Payment.Success) return response;

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
        /// <param name="invoice">The invoice to be paid</param>
        /// <param name="amount">The amount of the payment to the invoice</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult AuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(invoice, "invoice");


            // persist the invoice
            if (!invoice.HasIdentity)
                GatewayProviderService.Save(invoice);

            // authorize and capture the payment
            var response = PerformAuthorizeCapturePayment(invoice, amount, args);

            AuthorizeCaptureAttempted.RaiseEvent(new PaymentAttemptEventArgs<IPaymentResult>(response), this);

            if (!response.Payment.Success) return response;

            AssertPaymentApplied(response, invoice);

            AssertInvoiceStatus(invoice);

            // Check configuration for override on ApproveOrderCreation
            if (!response.ApproveOrderCreation)
                ((PaymentResult)response).ApproveOrderCreation = MerchelloConfiguration.Current.AlwaysApproveOrderCreation;

            // give response
            return response;
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be paid</param>
        /// <param name="payment">The payment to capture</param>
        /// <param name="amount">The amount of the payment to be captured</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult CapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(invoice, "invoice");

            // persist the invoice
            if (!invoice.HasIdentity)
                GatewayProviderService.Save(invoice);

            var response = PerformCapturePayment(invoice, payment, amount, args);

            // Special case where the order has already been created due to configuration override.
            if (invoice.Orders.Any() && response.ApproveOrderCreation)
                ((PaymentResult)response).ApproveOrderCreation = false;

            CaptureAttempted.RaiseEvent(new PaymentAttemptEventArgs<IPaymentResult>(response), this);

            if (!response.Payment.Success) return response;

            AssertPaymentApplied(response, invoice);

            AssertInvoiceStatus(invoice);


            // give response
            return response;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="amount">The amount to be refunded</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult RefundPayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(invoice, "invoice");
            if (!invoice.HasIdentity) return new PaymentResult(Attempt<IPayment>.Fail(new InvalidOperationException("Cannot refund a payment on an invoice that cannot have payments")), invoice, false);

            var response = PerformRefundPayment(invoice, payment, amount, args);

            RefundAttempted.RaiseEvent(new PaymentAttemptEventArgs<IPaymentResult>(response), this);

            if (!response.Payment.Success) return response;

            var appliedPayments = payment.AppliedPayments().Where(x => x.TransactionType != AppliedPaymentType.Void);
            foreach (var appliedPayment in appliedPayments)
            {
                appliedPayment.TransactionType = AppliedPaymentType.Void;
                appliedPayment.Amount = 0;

                GatewayProviderService.Save(appliedPayment);
            }

            AssertInvoiceStatus(invoice);

            // Force the ApproveOrderCreation to false
            if (response.ApproveOrderCreation) ((PaymentResult)response).ApproveOrderCreation = false;

            // give response
            return response;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="invoice">The invoice associated with the payment to be voided</param>
        /// <param name="payment">The payment to be voided</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult VoidPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(invoice, "invoice");
            if (!invoice.HasIdentity) return new PaymentResult(Attempt<IPayment>.Fail(new InvalidOperationException("Cannot void a payment on an invoice that cannot have payments")), invoice, false);

            var response = PerformVoidPayment(invoice, payment, args);

            VoidAttempted.RaiseEvent(new PaymentAttemptEventArgs<IPaymentResult>(response), this);

            if (!response.Payment.Success) return response;

            var appliedPayments = payment.AppliedPayments().Where(x => x.TransactionType != AppliedPaymentType.Void);
            foreach (var appliedPayment in appliedPayments)
            {
                appliedPayment.TransactionType = AppliedPaymentType.Void;
                appliedPayment.Amount = 0;

                GatewayProviderService.Save(appliedPayment);
            }

            AssertInvoiceStatus(invoice);

            // Assert the payment has been voided
            if (!payment.Voided)
            {
                payment.Voided = true;
                GatewayProviderService.Save(payment);
            }

            // Force the ApproveOrderCreation to false
            if (response.ApproveOrderCreation)
                ((PaymentResult)response).ApproveOrderCreation = false;

            // give response
            return response;
        }
        

        /// <summary>
        /// Does the actual work of creating and authorizing the payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="args">Any arguments required to process the payment. (Maybe a username, password or some API Key)</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected abstract IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args);

        /// <summary>
        /// Doe the actual work of Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The invoice to be paid</param>
        /// <param name="amount">The amount of the payment to the invoice</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        protected abstract IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args);

        /// <summary>
        /// Does the actual work of capturing a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="payment">The payment to be captured</param>
        /// <param name="amount">The amount of the payment to be captured</param>
        /// <param name="args">Any arguments required to process the payment. (Maybe a username, password or some API Key)</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected abstract IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args);


        /// <summary>
        /// Does the actual work of refunding the payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="amount">The amount of the payment to be refunded</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        protected abstract IPaymentResult PerformRefundPayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args);

        /// <summary>
        /// Does the actual work of voiding a payment
        /// </summary>
        /// <param name="invoice">The invoice to which the payment is associated</param>
        /// <param name="payment">The payment to be voided</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        protected abstract IPaymentResult PerformVoidPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args);

        /// <summary>
        /// Provides the assertion that the payment is applied to the invoice
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        private void AssertPaymentApplied(IPaymentResult response, IInvoice invoice)
        {
            // Apply the payment to the invoice if it was not done in the sub class           
            var payment = response.Payment.Result;
            if (payment.AppliedPayments(GatewayProviderService).FirstOrDefault(x => x.InvoiceKey == invoice.Key) == null)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, PaymentMethod.Name, payment.Amount);
            }
        }

        /// <summary>
        /// Validates the invoice status is correct after a payment request
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        private void AssertInvoiceStatus(IInvoice invoice)
        {
            var appliedPayments = _gatewayProviderService.GetAppliedPaymentsByInvoiceKey(invoice.Key).ToArray();
            
            var appliedTotal = 
                appliedPayments.Where(x => x.TransactionType == AppliedPaymentType.Debit).Sum(x => x.Amount) - 
                appliedPayments.Where(x => x.TransactionType == AppliedPaymentType.Credit).Sum(x => x.Amount);

            var statuses = GatewayProviderService.GetAllInvoiceStatuses().ToArray();

            if (appliedTotal == 0 && invoice.InvoiceStatusKey != Core.Constants.DefaultKeys.InvoiceStatus.Unpaid)
                invoice.InvoiceStatus = statuses.First(x => x.Key == Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);
            if (invoice.Total <= appliedTotal && invoice.InvoiceStatusKey != Core.Constants.DefaultKeys.InvoiceStatus.Paid)
                invoice.InvoiceStatus = statuses.First(x => x.Key == Core.Constants.DefaultKeys.InvoiceStatus.Paid);
            if (invoice.Total > appliedTotal && invoice.InvoiceStatusKey != Core.Constants.DefaultKeys.InvoiceStatus.Partial)
                invoice.InvoiceStatus = statuses.First(x => x.Key == Core.Constants.DefaultKeys.InvoiceStatus.Partial);

            if(invoice.IsDirty()) GatewayProviderService.Save(invoice);
                
        }        
    }
}