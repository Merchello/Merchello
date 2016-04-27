namespace Merchello.Providers.Payment.PayPal
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.Braintree.Controllers;
    using Merchello.Providers.Payment.PayPal.Models;
    using Merchello.Providers.Payment.PayPal.Provider;

    using Constants = Merchello.Providers.Constants;

    /// <summary>
    /// Utility class that assists in PayPal API calls.
    /// </summary>
    public class PayPalApiHelper
    {
        /// <summary>
        /// Gets the <see cref="PayPalProviderSettings"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="PayPalProviderSettings"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the PayPalGatewayProvider has not been activated
        /// </exception>
        public static PayPalProviderSettings GetPayPalProviderSettings()
        {
            var provider = (PayPalPaymentGatewayProvider)MerchelloContext.Current.Gateways.Payment.GetProviderByKey(Constants.PayPal.GatewayProviderSettingsKey);

            if (provider != null) return provider.ExtendedData.GetPayPalProviderSettings();

            var logData = MultiLogger.GetBaseLoggingData();
            logData.AddCategory("GatewayProviders");
            logData.AddCategory("PayPal");

            var ex = new NullReferenceException("The PayPalPaymentGatewayProvider could not be resolved.  The provider must be activiated");
            MultiLogHelper.Error<BraintreeApiController>("PayPalPaymentGatewayProvider not activated.", ex, logData);
            throw ex;
        } 
    }
}