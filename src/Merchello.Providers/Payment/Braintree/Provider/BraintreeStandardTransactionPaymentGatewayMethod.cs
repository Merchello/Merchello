namespace Merchello.Providers.Payment.Braintree.Provider
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Providers.Payment.Braintree.Services;
    using Merchello.Providers.Payment.Exceptions;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    using Constants = Merchello.Providers.Constants;
    using IInvoice = Merchello.Core.Models.IInvoice;
    using IPaymentMethod = Merchello.Core.Models.IPaymentMethod;

    /// <summary>
    /// Represents the BraintreePaymentGatewayMethod
    /// </summary>
    [GatewayMethodUi("BrainTree.StandardTransaction")]
    [GatewayMethodEditor("BrainTree Payment Method Editor", "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.paymentmethod.addedit.html")]
    [PaymentGatewayMethod("Braintree Payment Gateway Method Editors",
        "~/App_Plugins/MerchelloProviders/views/dialogs/braintree.standard.authorizepayment.html",
        "~/App_Plugins/MerchelloProviders/views/dialogs/braintree.standard.authorizecapturepayment.html",
        "~/App_Plugins/MerchelloProviders/views/dialogs/braintree.standard.voidpayment.html",
        "~/App_Plugins/MerchelloProviders/views/dialogs/braintree.standard.refundpayment.html",
        "~/App_Plugins/MerchelloProviders/views/dialogs/braintree.standard.capturepayment.html")]
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
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the payment line authorize description.
        /// </summary>
        protected override string PaymentLineAuthorizeDescription { get; set; }

        /// <summary>
        /// Gets or sets the payment line authorize capture description.
        /// </summary>
        protected override string PaymentLineAuthorizeCaptureDescription { get; set; }


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
            
            var attempt = this.ProcessPayment(invoice, TransactionOption.Authorize, invoice.Total, paymentMethodNonce);

            var payment = attempt.Payment.Result;

            this.GatewayProviderService.Save(payment);

            if (!attempt.Payment.Success)
            {
                this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, attempt.Payment.Exception.Message, 0);
            }
            else
            {
                this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, this.PaymentLineAuthorizeDescription, 0);
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

            var attempt = this.ProcessPayment(invoice, TransactionOption.SubmitForSettlement, invoice.Total, paymentMethodNonce, email);

            var payment = attempt.Payment.Result;

            this.GatewayProviderService.Save(payment);

            if (!attempt.Payment.Success)
            {
                this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, attempt.Payment.Exception.Message, 0);
            }
            else
            {
                this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, this.PaymentLineAuthorizeCaptureDescription, amount);
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
        /// This converts the <see cref="Result{T}"/> into Merchello <see cref="IPaymentResult"/>
        /// </remarks>
        protected override IPaymentResult ProcessPayment(IInvoice invoice, TransactionOption option, decimal amount, string token, string email = "")
        {
            var payment = this.GatewayProviderService.CreatePayment(PaymentMethodType.CreditCard, amount, this.PaymentMethod.Key);

            payment.CustomerKey = invoice.CustomerKey;
            payment.Authorized = false;
            payment.Collected = false;
            payment.PaymentMethodName = this.BackOfficePaymentMethodName;
            payment.ExtendedData.SetValue(Constants.Braintree.ProcessorArguments.PaymentMethodNonce, token);

            var result = this.BraintreeApiService.Transaction.Sale(invoice, token, option: option, email: email);

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

        /// <summary>
        /// Initializes the gateway method.
        /// </summary>
        private void Initialize()
        {
            this.PaymentLineAuthorizeDescription = "To show record of Braintree Authorization";
            this.PaymentLineAuthorizeCaptureDescription = "Braintree transaction - authorized and captured";
        }
    }
}