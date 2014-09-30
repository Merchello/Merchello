using System;
using System.Diagnostics;
using Braintree;
using Merchello.Core;
using Merchello.Plugin.Payments.Braintree.Exceptions;
using Merchello.Plugin.Payments.Braintree.Models;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Plugin.Payments.Braintree.Provider
{
    using Core.Gateways;
    using Core.Gateways.Payment;
    using Core.Models;
    using Core.Services;
    using Services;

    /// <summary>
    /// Represents the BraintreePaymentGatewayMethod
    /// </summary>
    [GatewayMethodUi("BrainTree.CreditCard")]
    [GatewayMethodEditor("BrainTree Payment Method Editor", "~/App_Plugins/Merchello.BrainTree/paymentmethod.html")]
    public class BraintreeSimpleTransactionPaymentGatewayMethod : PaymentGatewayMethodBase, IBraintreeSimpleTransactionPaymentGatewayMethod
    {
        /// <summary>
        /// The <see cref="IBraintreeApiService"/>
        /// </summary>
        private readonly IBraintreeApiService _braintreeApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeSimpleTransactionPaymentGatewayMethod"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <param name="braintreeApiService">
        /// The braintree Api Service.
        /// </param>
        public BraintreeSimpleTransactionPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, IBraintreeApiService braintreeApiService)
            : base(gatewayProviderService, paymentMethod)
        {
            Mandate.ParameterNotNull(braintreeApiService, "braintreeApiService");

            _braintreeApiService = braintreeApiService;
        }

        /// <summary>
        /// Does the actual work of authorizing the payment
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            var paymentMethodNonce = args.GetPaymentMethodNonce();

            if (string.IsNullOrEmpty(paymentMethodNonce))
            {
                var error = new InvalidOperationException("No payment method nonce was found in the ProcessorArgumentCollection");
                LogHelper.Debug<BraintreeSimpleTransactionPaymentGatewayMethod>(error.Message);
                return new PaymentResult(Attempt<IPayment>.Fail(error), invoice, false);
            }
            
            var attempt = ProcessPayment(invoice, TransactionOption.Authorize, invoice.Total, paymentMethodNonce);

            var payment = attempt.Payment.Result;

            GatewayProviderService.Save(payment);

            if (!attempt.Payment.Success)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, attempt.Payment.Exception.Message, 0);
            }
            else
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "To show record of Braintree Authorization", 0);
            }

            return attempt;
        }

        /// <summary>
        /// Performs the actual work of authorizing and capturing a payment.  
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        /// <remarks>
        /// This is a transaction with SubmitForSettlement = true
        /// </remarks>
        protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {
            var paymentMethodNonce = args.GetPaymentMethodNonce();

            if (string.IsNullOrEmpty(paymentMethodNonce))
            {
                var error = new InvalidOperationException("No payment method nonce was found in the ProcessorArgumentCollection");
                LogHelper.Debug<BraintreeSimpleTransactionPaymentGatewayMethod>(error.Message);
                return new PaymentResult(Attempt<IPayment>.Fail(error), invoice, false);
            }

            var attempt = ProcessPayment(invoice, TransactionOption.Authorize, invoice.Total, paymentMethodNonce);

            var payment = attempt.Payment.Result;

            GatewayProviderService.Save(payment);

            if (!attempt.Payment.Success)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, attempt.Payment.Exception.Message, 0);
            }
            else
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "Braintree transaction - authorized and captured", amount);
            }

            return attempt;
        }

        protected override IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            throw new System.NotImplementedException();
        }

        protected override IPaymentResult PerformRefundPayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Performs the actual work of voiding a payment in Merchello ONLY
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        /// <remarks>
        /// This merely VOIDs the payment in Merchello and does nothing against the Braintree API as Braintree does not do a VOID
        /// </remarks>
        protected override IPaymentResult PerformVoidPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Processes a payment against the Braintree API using the BraintreeApiService.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        /// <remarks>
        /// This converts the <see cref="Result{Transaction}"/> into Merchello's <see cref="IPaymentResult"/>
        /// </remarks>
        private IPaymentResult ProcessPayment(IInvoice invoice, TransactionOption option, decimal amount, string paymentMethodNonce)
        {
            var payment = GatewayProviderService.CreatePayment(PaymentMethodType.CreditCard, amount, PaymentMethod.Key);

            payment.CustomerKey = invoice.CustomerKey;
            payment.Authorized = false;
            payment.Collected = false;
            payment.PaymentMethodName = "Braintree Transaction";
            payment.ExtendedData.SetValue(Constants.ProcessorArguments.PaymentMethodNonce, paymentMethodNonce);

            var result = _braintreeApiService.Transaction.Sale(invoice, paymentMethodNonce, option: option);

            if (result.IsSuccess())
            {
                payment.ExtendedData.SetBraintreeTransaction(result.Target);

                if (option == TransactionOption.Authorize) payment.Authorized = true;
                if (option == TransactionOption.SubmitForSettlement)
                {
                    payment.Authorized = true;
                    payment.Collected = true;
                }   
          

                return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
            }

            var error = new BraintreeApiException(result.Errors, result.Message);

            return new PaymentResult(Attempt<IPayment>.Fail(payment, error), invoice, false);
        }        
    }
}