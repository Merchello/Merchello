﻿namespace Merchello.Plugin.Payments.Braintree.Provider
{
    using System;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Plugin.Payments.Braintree.Exceptions;
    using Merchello.Plugin.Payments.Braintree.Models;
    using Merchello.Plugin.Payments.Braintree.Services;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents the BraintreePaymentGatewayMethod
    /// </summary>
    [GatewayMethodUi("BrainTree.StandardTransaction")]
    [GatewayMethodEditor("BrainTree Payment Method Editor", "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.paymentmethod.addedit.html")]
    [PaymentGatewayMethod("Braintree Payment Gateway Method Editors",
        "~/App_Plugins/Merchello.Braintree/dialogs/payment.standard.authorizepayment.html",
        "~/App_Plugins/Merchello.Braintree/dialogs/payment.standard.authorizecapturepayment.html",
        "~/App_Plugins/Merchello.Braintree/dialogs/payment.standard.voidpayment.html",
        "~/App_Plugins/Merchello.Braintree/dialogs/payment.standard.refundpayment.html",
        "~/App_Plugins/Merchello.Braintree/dialogs/payment.standard.capturepayment.html")]
    public class BraintreeStandardTransactionPaymentGatewayMethod : BraintreePaymentGatewayMethodBase, IBraintreeStandardTransactionPaymentGatewayMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeStandardTransactionPaymentGatewayMethod"/> class.
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
        public BraintreeStandardTransactionPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, IBraintreeApiService braintreeApiService)
            : base(gatewayProviderService, paymentMethod, braintreeApiService)
        {
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
                LogHelper.Debug<BraintreeStandardTransactionPaymentGatewayMethod>(error.Message);
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
                LogHelper.Debug<BraintreeStandardTransactionPaymentGatewayMethod>(error.Message);
                return new PaymentResult(Attempt<IPayment>.Fail(error), invoice, false);
            }

            // TODO this is a total last minute hack
            var email = string.Empty;
            if (args.ContainsKey("customerEmail")) email = args["customerEmail"];

            var attempt = ProcessPayment(invoice, TransactionOption.SubmitForSettlement, invoice.Total, paymentMethodNonce, email);

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
        /// <param name="token">
        /// The payment method nonce.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        /// <remarks>
        /// This converts the <see cref="Result{T}"/> into Merchello's <see cref="IPaymentResult"/>
        /// </remarks>
        protected override IPaymentResult ProcessPayment(IInvoice invoice, TransactionOption option, decimal amount, string token, string email = "")
        {
            var payment = GatewayProviderService.CreatePayment(PaymentMethodType.CreditCard, amount, PaymentMethod.Key);

            payment.CustomerKey = invoice.CustomerKey;
            payment.Authorized = false;
            payment.Collected = false;
            payment.PaymentMethodName = "Braintree Transaction";
            payment.ExtendedData.SetValue(Braintree.Constants.ProcessorArguments.PaymentMethodNonce, token);

            var result = BraintreeApiService.Transaction.Sale(invoice, token, option: option, email: email);

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