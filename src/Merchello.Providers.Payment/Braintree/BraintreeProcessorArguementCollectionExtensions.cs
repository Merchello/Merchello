namespace Merchello.Providers.Payment.Braintree
{
    using Merchello.Core.Gateways.Payment;

    using Umbraco.Core.Logging;

    /// <summary>
    /// Braintree ProcessorArgumentCollection utility extensions.
    /// </summary>
    public static class BraintreeProcessorArguementCollectionExtensions
    {
        /// <summary>
        /// Sets the payment method nonce.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        public static void SetPaymentMethodNonce(this ProcessorArgumentCollection args, string paymentMethodNonce)
        {
            args.Add(Constants.Braintree.ProcessorArguments.PaymentMethodNonce, paymentMethodNonce);
        }

        /// <summary>
        /// Gets the payment method nonce from the <see cref="ProcessorArgumentCollection"/>
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The payment method nonce.
        /// </returns>
        public static string GetPaymentMethodNonce(this ProcessorArgumentCollection args)
        {
            if (args.ContainsKey(Constants.Braintree.ProcessorArguments.PaymentMethodNonce)) return args[Constants.Braintree.ProcessorArguments.PaymentMethodNonce];

            LogHelper.Debug(typeof(MappingExtensions), "Payment Method Nonce not found in process argument collection");

            return string.Empty;
        }

        /// <summary>
        /// The set payment method token.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="paymentMethodToken">
        /// The payment method token.
        /// </param>
        public static void SetPaymentMethodToken(this ProcessorArgumentCollection args, string paymentMethodToken)
        {
            args.Add(Constants.Braintree.ProcessorArguments.PaymentMethodToken, paymentMethodToken);
        }

        /// <summary>
        /// The get payment method token.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetPaymentMethodToken(this ProcessorArgumentCollection args)
        {
            if (args.ContainsKey(Constants.Braintree.ProcessorArguments.PaymentMethodToken)) return args[Constants.Braintree.ProcessorArguments.PaymentMethodToken];

            LogHelper.Debug(typeof(MappingExtensions), "Payment Method Token not found in process argument collection");

            return string.Empty;
        }
    }
}