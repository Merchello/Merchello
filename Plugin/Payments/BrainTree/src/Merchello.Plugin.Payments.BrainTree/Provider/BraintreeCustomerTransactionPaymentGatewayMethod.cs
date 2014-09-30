namespace Merchello.Plugin.Payments.Braintree.Provider
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Plugin.Payments.Braintree.Exceptions;
    using Merchello.Plugin.Payments.Braintree.Models;
    using Merchello.Plugin.Payments.Braintree.Services;

    using Umbraco.Core;

    /// <summary>
    /// Represents a BraintreeCustomerTransactionPaymentGatewayMethod
    /// </summary>
    /// <remarks>
    /// This method assumes that the invoice is associated with a customer
    /// </remarks>
    [GatewayMethodUi("BrainTree.CustomerTransaction")]
    [GatewayMethodEditor("BrainTree Payment Method Editor", "~/App_Plugins/Merchello.BrainTree/paymentmethod.html")]
    public class BraintreeCustomerTransactionPaymentGatewayMethod : BraintreePaymentGatewayMethodBase,  IBraintreeCustomerTransactionPaymentGatewayMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeCustomerTransactionPaymentGatewayMethod"/> class.
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
        public BraintreeCustomerTransactionPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, IBraintreeApiService braintreeApiService) 
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
            var customer = invoice.Customer();

            throw new NotImplementedException();
        }

        protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {
            throw new NotImplementedException();
        }

        protected override IPaymentResult ProcessPayment(IInvoice invoice, TransactionOption option, decimal amount, string paymentMethodNonce)
        {
            var payment = GatewayProviderService.CreatePayment(PaymentMethodType.CreditCard, amount, PaymentMethod.Key);

            payment.CustomerKey = invoice.CustomerKey;
            payment.Authorized = false;
            payment.Collected = false;
            payment.PaymentMethodName = "Braintree Transaction";
            payment.ExtendedData.SetValue(Braintree.Constants.ProcessorArguments.PaymentMethodNonce, paymentMethodNonce);


            var customer = invoice.Customer();

            if (customer == null)
            {
                var customerError = new NullReferenceException("A customer is not associated with the invoice");
                return new PaymentResult(Attempt<IPayment>.Fail(payment, customerError), invoice, false);
            }

            var result = BraintreeApiService.Transaction.Sale(invoice, paymentMethodNonce, customer, invoice.GetBillingAddress(), option);

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