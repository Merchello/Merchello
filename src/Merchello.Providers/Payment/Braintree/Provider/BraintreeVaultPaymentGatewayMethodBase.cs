namespace Merchello.Providers.Payment.Braintree.Provider
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Providers.Payment.Braintree.Services;
    using Merchello.Providers.Payment.Exceptions;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    using Constants = Merchello.Providers.Constants;

    /// <summary>
    /// The braintree vault payment gateway method base.
    /// </summary>
    public abstract class BraintreeVaultPaymentGatewayMethodBase : BraintreePaymentGatewayMethodBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeVaultPaymentGatewayMethodBase"/> class.
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
        protected BraintreeVaultPaymentGatewayMethodBase(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, IBraintreeApiService braintreeApiService)
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
            if (this.BraintreeApiService.BraintreeProviderSettings.DefaultTransactionOption == TransactionOption.SubmitForSettlement)
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

            var attempt = this.ProcessPayment(invoice, TransactionOption.Authorize, invoice.Total, paymentMethodToken);

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

            var attempt = this.ProcessPayment(invoice, TransactionOption.SubmitForSettlement, invoice.Total, paymentMethodToken);

            var payment = attempt.Payment.Result;

            this.GatewayProviderService.Save(payment);

            if (!attempt.Payment.Success)
            {
                this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, attempt.Payment.Exception.Message, 0);
            }
            else
            {
                var customerKey = invoice.CustomerKey.GetValueOrDefault();
                var last4 = string.Empty;
                if (!Guid.Empty.Equals(customerKey))
                {
                    var customer = this.BraintreeApiService.Customer.GetBraintreeCustomer(customerKey);
                    if (customer.CreditCards.Any())
                    {
                        var cc = customer.CreditCards.FirstOrDefault(x => x.Token == paymentMethodToken);
                        if (cc != null)
                        {
                            last4 += " - " + cc.CardType + " " + cc.LastFour;
                        }
                    }
                }

                this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, this.PaymentLineAuthorizeCaptureDescription + " " + last4, payment.Amount);
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
            var payment = this.GatewayProviderService.CreatePayment(PaymentMethodType.CreditCard, amount, this.PaymentMethod.Key);

            payment.CustomerKey = invoice.CustomerKey;
            payment.Authorized = false;
            payment.Collected = false;
            payment.PaymentMethodName = this.BackOfficePaymentMethodName;
            payment.ExtendedData.SetValue(Constants.Braintree.ProcessorArguments.PaymentMethodNonce, token);

            var result = this.BraintreeApiService.Transaction.VaultSale(invoice, token, option);

            if (result.IsSuccess())
            {
                payment.ExtendedData.SetBraintreeTransaction(result.Target);

                // AVS and CVV data
                payment.ExtendedData.SetAvsCvvData(result.Target);

                // Set the transaction ID as an extended data item
                payment.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.TransactionId, result.Target.Id);

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