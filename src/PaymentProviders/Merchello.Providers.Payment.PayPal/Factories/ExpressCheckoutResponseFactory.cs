namespace Merchello.Providers.Payment.PayPal.Factories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Logging;
    using Merchello.Providers.Payment.PayPal.Models;
    using Merchello.Providers.Payment.PayPal.Services;

    using global::PayPal.PayPalAPIInterfaceService.Model;

    /// <summary>
    /// A factory responsible for building <see cref="PayPalExpressTransaction"/>.
    /// </summary>
    internal class ExpressCheckoutResponseFactory
    {
        /// <summary>
        /// Gets the <see cref="ExpressCheckoutResponse"/> from PayPal's <see cref="AbstractResponseType"/>.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="ExpressCheckoutResponse"/>.
        /// </returns>
        public ExpressCheckoutResponse Build(AbstractResponseType response, string token = "")
        {
            return new ExpressCheckoutResponse
            {
                Ack = response.Ack,
                Build = response.Build,
                ErrorTypes = response.Errors,
                Token = token,
                Version = response.Version
            };
        }

        /// <summary>
        /// Constructs an error response message from an exception.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns>
        /// The <see cref="ExpressCheckoutResponse"/>.
        /// </returns>
        public ExpressCheckoutResponse Build(Exception ex)
        {
            var logData = MultiLogger.GetBaseLoggingData();
            logData.AddCategory("GatewayProviders");
            logData.AddCategory("PayPal");

            // bubble up the error
            var errorType = new ErrorType()
            {
                SeverityCode = SeverityCodeType.CUSTOMCODE,
                ShortMessage = ex.Message,
                LongMessage = ex.Message,
                ErrorCode = "PPEService"
            };

            logData.SetValue("payPalErrorType", errorType);

            MultiLogHelper.Error<PayPalExpressCheckoutService>("Failed to get response from PayPalAPIInterfaceServiceService", ex, logData);

            return new ExpressCheckoutResponse { Ack = AckCodeType.CUSTOMCODE, ErrorTypes = new List<ErrorType> { errorType } };
        }

    }
}