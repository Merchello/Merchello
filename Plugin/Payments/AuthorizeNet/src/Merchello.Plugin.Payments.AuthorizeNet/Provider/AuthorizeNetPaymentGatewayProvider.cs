namespace Merchello.Plugin.Payments.AuthorizeNet.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Gateways;
    using Core.Gateways.Payment;
    using Core.Models;
    using Core.Services;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The authorize net payment gateway provider.
    /// </summary>
    [GatewayProviderActivation("C6BF6743-3565-401F-911A-33B68CACB11B", "AuthorizeNet Payment Provider", "AuthorizeNet Payment Provider")]
    [GatewayProviderEditor("AuthorizeNet configuration", "~/App_Plugins/Merchello.AuthorizeNet/editor.html")]
    public class AuthorizeNetPaymentGatewayProvider : PaymentGatewayProviderBase, IAuthorizeNetPaymentGatewayProvider
    {
        #region AvailableResources

        /// <summary>
        /// The available resources.
        /// </summary>
        internal static readonly IEnumerable<IGatewayResource> AvailableResources = new List<IGatewayResource>()
        {
            new GatewayResource("CreditCard", "Credit Card")
        };

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeNetPaymentGatewayProvider"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="gatewayProviderSettings">
        /// The gateway provider settings.
        /// </param>
        /// <param name="runtimeCacheProvider">
        /// The runtime cache provider.
        /// </param>
        public AuthorizeNetPaymentGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {
        }

        /// <summary>
        /// Returns a list of remaining available resources
        /// </summary>
        /// <returns>
        /// The collection of <see cref="IGatewayResource"/>.
        /// </returns>
        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            // PaymentMethods is created in PaymentGatewayProviderBase.  It is a list of all previously saved payment methods
            return AvailableResources.Where(x => PaymentMethods.All(y => y.PaymentCode != x.ServiceCode));
        }

        /// <summary>
        /// Creates a <see cref="IPaymentGatewayMethod"/>
        /// </summary>
        /// <param name="gatewayResource">
        /// The gateway Resource.
        /// </param>
        /// <param name="name">
        /// The name of the payment method
        /// </param>
        /// <param name="description">
        /// The description of the payment method
        /// </param>
        /// <returns>
        /// A <see cref="IPaymentGatewayMethod"/>
        /// </returns>
        public override IPaymentGatewayMethod CreatePaymentMethod(IGatewayResource gatewayResource, string name, string description)
        {
            // assert gateway resource is still available
            var available = ListResourcesOffered().FirstOrDefault(x => x.ServiceCode == gatewayResource.ServiceCode);
            if(available == null) throw new InvalidOperationException("GatewayResource has already been assigned");

            var attempt = GatewayProviderService.CreatePaymentMethodWithKey(GatewayProviderSettings.Key, name, description, available.ServiceCode);            
    
            if (attempt.Success)
            {
                PaymentMethods = null;

                return new AuthorizeNetPaymentGatewayMethod(GatewayProviderService, attempt.Result, GatewayProviderSettings.ExtendedData);
            }

            LogHelper.Error<AuthorizeNetPaymentGatewayProvider>(string.Format("Failed to create a payment method name: {0}, description {1}, paymentCode {2}", name, description, available.ServiceCode), attempt.Exception);

            throw attempt.Exception;
        }

        /// <summary>
        /// Gets a <see cref="IPaymentGatewayMethod"/> by it's unique 'key'
        /// </summary>
        /// <param name="paymentMethodKey">The key of the <see cref="IPaymentMethod"/></param>
        /// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
        public override IPaymentGatewayMethod GetPaymentGatewayMethodByKey(Guid paymentMethodKey)
        {
            var paymentMethod = PaymentMethods.FirstOrDefault(x => x.Key == paymentMethodKey);

            if (paymentMethod == null) throw new NullReferenceException("PaymentMethod not found");

            return new AuthorizeNetPaymentGatewayMethod(GatewayProviderService, paymentMethod, GatewayProviderSettings.ExtendedData);

        }

        /// <summary>
        /// Gets a <see cref="IPaymentGatewayMethod"/> by it's payment code
        /// </summary>
        /// <param name="paymentCode">The payment code of the <see cref="IPaymentGatewayMethod"/></param>
        /// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
        public override IPaymentGatewayMethod GetPaymentGatewayMethodByPaymentCode(string paymentCode)
        {
            var paymentMethod = PaymentMethods.FirstOrDefault(x => x.PaymentCode == paymentCode);

            if (paymentMethod == null) throw new NullReferenceException("PaymentMethod not found");

            return new AuthorizeNetPaymentGatewayMethod(GatewayProviderService, paymentMethod, GatewayProviderSettings.ExtendedData);
        }
    }
}