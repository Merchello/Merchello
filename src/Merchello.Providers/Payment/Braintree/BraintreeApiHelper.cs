namespace Merchello.Providers.Payment.Braintree
{
    using System;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.Braintree.Controllers;
    using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Providers.Payment.Braintree.Provider;
    using Merchello.Providers.Payment.Models;

    using Umbraco.Core.Logging;

    using Constants = Merchello.Providers.Constants;

    /// <summary>
    /// Utility class that assists in Braintree API calls
    /// </summary>
    public class BraintreeApiHelper
    {
        /// <summary>
        /// Gets the Merchello <see cref="ICustomer"/> for a given Braintree <see cref="Transaction"/>.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomer"/>.
        /// </returns>
        public static ICustomer GetCustomerForTransaction(Transaction transaction)
        {
            return GetCustomerForTransaction(MerchelloContext.Current, transaction);
        }

        /// <summary>
        /// Gets the Merchello <see cref="ICustomer"/> for a given Braintree <see cref="Transaction"/>.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomer"/>.
        /// </returns>
        public static ICustomer GetCustomerForTransaction(IMerchelloContext merchelloContext, Transaction transaction)
        {
            var customerService = merchelloContext.Services.CustomerService;

            return customerService.GetByKey(new Guid(transaction.Customer.Id));
        }

        /// <summary>
        /// Gets the <see cref="BraintreeProviderSettings"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="BraintreeProviderSettings"/>.
        /// </returns>
        /// <remarks>
        /// This is only used by <see cref="BraintreeWebhooksControllerBase"/>
        /// </remarks>
        internal static BraintreeProviderSettings GetBraintreeProviderSettings()
        {
            var provider = (BraintreePaymentGatewayProvider)MerchelloContext.Current.Gateways.Payment.GetProviderByKey(Constants.Braintree.GatewayProviderSettingsKey);

            if (provider != null) return provider.ExtendedData.GetBrainTreeProviderSettings();

            var ex = new NullReferenceException("The BraintreePaymentGatewayProvider could not be resolved.  The provider must be activiated");
            LogHelper.Error<BraintreeApiController>("BraintreePaymentGatewayProvider not activated.", ex);
            throw ex;
        }
    }
}