namespace Merchello.Providers.Payment.PurchaseOrder.Provider
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
    /// The purchase order payment gateway provider.
    /// </summary>
    [GatewayProviderActivation("f8fa58d9-2c0d-4e07-aaac-e0f86f5b622e", "Purchase Order Payment Provider", "Purchase Order Payment Provider")] 
    public class PurchaseOrderPaymentGatewayProvider : PaymentGatewayProviderBase, IPurchaseOrderPaymentGatewayProvider
    {
        #region AvailableResources

        /// <summary>
        /// The available resources.
        /// </summary>
        internal static readonly IEnumerable<IGatewayResource> AvailableResources = new List<IGatewayResource>
        {
            new GatewayResource("PurchaseOrder", "Purchase Order")
        };

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseOrderPaymentGatewayProvider"/> class.
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
        public PurchaseOrderPaymentGatewayProvider(IGatewayProviderService gatewayProviderService, 
            IGatewayProviderSettings gatewayProviderSettings, 
            IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {
        }

        /// <summary>
        /// Creates a <see cref="IPaymentGatewayMethod"/>
        /// </summary>
        /// <param name="name">The name of the payment method</param>
        /// <param name="description">The description of the payment method</param>
        /// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
        public IPaymentGatewayMethod CreatePaymentMethod(string name, string description)
        {
            return CreatePaymentMethod(AvailableResources.First(), name, description);
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
            var paymentCode = gatewayResource.ServiceCode + "-" + Guid.NewGuid();

            var attempt = GatewayProviderService.CreatePaymentMethodWithKey(GatewayProviderSettings.Key, name, description, paymentCode);

            if (attempt.Success)
            {
                PaymentMethods = null;

                return new PurchaseOrderPaymentGatewayMethod(GatewayProviderService, attempt.Result);
            }

            LogHelper.Error<PurchaseOrderPaymentGatewayProvider>(string.Format("Failed to create a payment method name: {0}, description {1}, paymentCode {2}", name, description, paymentCode), attempt.Exception);

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

            return new PurchaseOrderPaymentGatewayMethod(GatewayProviderService, paymentMethod);

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

            return new PurchaseOrderPaymentGatewayMethod(GatewayProviderService, paymentMethod);
        }

        /// <summary>
        /// Returns a list of remaining available resources
        /// </summary>
        /// <returns>
        /// The collection of <see cref="IGatewayResource"/>.
        /// </returns>
        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            return AvailableResources;
        }

    }
}