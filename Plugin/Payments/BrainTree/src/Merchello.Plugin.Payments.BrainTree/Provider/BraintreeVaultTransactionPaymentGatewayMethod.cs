namespace Merchello.Plugin.Payments.Braintree.Provider
{
    using System;
    using System.Linq;

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
    /// Represents a BraintreeVaultTransactionPaymentGatewayMethod
    /// </summary>
    /// <remarks>
    /// This method assumes that the invoice is associated with a customer
    /// </remarks>
    [GatewayMethodUi("BrainTree.CustomerTransaction")]
    [GatewayMethodEditor("BrainTree Payment Method Editor", "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.paymentmethod.addedit.html")]
    [PaymentGatewayMethod("Braintree Payment Gateway Method Editors",
    "~/App_Plugins/Merchello.Braintree/vault/payment.vault.authorizecapturepayment.html",
    "~/App_Plugins/Merchello.Braintree/vault/payment.vault.voidpayment.html",
    "~/App_Plugins/Merchello.Braintree/vault/payment.vault.refundpayment.html")]
    public class BraintreeVaultTransactionPaymentGatewayMethod : BraintreePaymentGatewayMethodBase,  IBraintreeVaultTransactionPaymentGatewayMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeVaultTransactionPaymentGatewayMethod"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <param name="braintreeApiService">
        /// The braintree api service.
        /// </param>
        public BraintreeVaultTransactionPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, IBraintreeApiService braintreeApiService) 
            : base(gatewayProviderService, paymentMethod, braintreeApiService)
        {
        }

        /// <summary>
        /// The perform authorize payment.
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
            // The Provider settings 
            if (BraintreeApiService.BraintreeProviderSettings.DefaultTransactionOption == TransactionOption.SubmitForSettlement)
            {
                return this.PerformAuthorizeCapturePayment(invoice, invoice.Total, args);
            }

            var paymentMethodToken = args.GetPaymentMethodToken();

            if (string.IsNullOrEmpty(paymentMethodToken))
            {
                var error = new InvalidOperationException("No payment method token was found in the ProcessorArgumentCollection");
                LogHelper.Debug<BraintreeStandardTransactionPaymentGatewayMethod>(error.Message);
                return new PaymentResult(Attempt<IPayment>.Fail(error), invoice, false);
            }

            var attempt = ProcessPayment(invoice, TransactionOption.Authorize, invoice.Total, paymentMethodToken);

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
        /// The perform authorize capture payment.
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
        protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {
            var paymentMethodToken = args.GetPaymentMethodToken();

            if (string.IsNullOrEmpty(paymentMethodToken))
            {
                var error = new InvalidOperationException("No payment method token was found in the ProcessorArgumentCollection");
                LogHelper.Debug<BraintreeStandardTransactionPaymentGatewayMethod>(error.Message);
                return new PaymentResult(Attempt<IPayment>.Fail(error), invoice, false);
            }

            var attempt = ProcessPayment(invoice, TransactionOption.SubmitForSettlement, invoice.Total, paymentMethodToken);

            var payment = attempt.Payment.Result;

            GatewayProviderService.Save(payment);

            if (!attempt.Payment.Success)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, attempt.Payment.Exception.Message, 0);
            }
            else
            {
                var customerKey = invoice.CustomerKey.GetValueOrDefault();
                var last4 = string.Empty;
                if (!Guid.Empty.Equals(customerKey))
                {
                    var customer = BraintreeApiService.Customer.GetBraintreeCustomer(customerKey);
                    if (customer.CreditCards.Any())
                    {
                        var cc = customer.CreditCards.FirstOrDefault(x => x.Token == paymentMethodToken);
                        if (cc != null)
                        {
                            last4 += " - " + cc.CardType + " " + cc.LastFour;
                        }
                    }
                }

                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "Braintree Vault Transaction" + last4, payment.Amount);
            }

            return attempt;
        }

        /// <summary>
        /// Processes the payment.
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
        /// The token.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        protected override IPaymentResult ProcessPayment(IInvoice invoice, TransactionOption option, decimal amount, string token, string email = "")
        {
            var payment = GatewayProviderService.CreatePayment(PaymentMethodType.CreditCard, amount, PaymentMethod.Key);

            payment.CustomerKey = invoice.CustomerKey;
            payment.Authorized = false;
            payment.Collected = false;
            payment.PaymentMethodName = "Braintree Vault Transaction";
            payment.ExtendedData.SetValue(Braintree.Constants.ProcessorArguments.PaymentMethodNonce, token);

            var result = BraintreeApiService.Transaction.VaultSale(invoice, token, option);

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