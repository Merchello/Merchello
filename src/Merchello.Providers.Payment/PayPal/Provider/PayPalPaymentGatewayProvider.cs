namespace Merchello.Providers.Payment.PayPal.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Providers.Payment.Models;

    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;

    using Constants = Payment.Constants;

    /// <summary>
	/// Represents a PayPalPaymentGatewayProvider
	/// </summary>
	[GatewayProviderActivation(Constants.PayPal.GatewayProviderKey, "PayPal Payment Provider", "PayPal Payment Provider")]
	[GatewayProviderEditor("PayPal configuration", "~/App_Plugins/Merchello.PayPal/editor.html")]
    [ProviderSettingsMapper(Constants.PayPal.ExtendedDataKeys.ProviderSettings, typeof(PayPalProviderSettings))]
    public class PayPalPaymentGatewayProvider : PaymentGatewayProviderBase
    {
		#region AvailableResources

        /// <summary>
        /// The available resources.
        /// </summary>
        internal static readonly IEnumerable<IGatewayResource> AvailableResources = new List<IGatewayResource>()
        {
            new GatewayResource("PayPal", "PayPal")
        };

		#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalPaymentGatewayProvider"/> class.
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
        public PayPalPaymentGatewayProvider(
            IGatewayProviderService gatewayProviderService,
            IGatewayProviderSettings gatewayProviderSettings,
            IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {
        }

        /// <summary>
        /// Returns a list of remaining available resources
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IGatewayResource> ListResourcesOffered()
		{
			// PaymentMethods is created in PaymentGatewayProviderBase.  It is a list of all previously saved payment methods
			return AvailableResources.Where(x => this.PaymentMethods.All(y => y.PaymentCode != x.ServiceCode));
		}

		/// <summary>
		/// Creates a <see cref="IPaymentGatewayMethod"/>
		/// </summary>
		/// <param name="gatewayResource"></param>
		/// <param name="name">The name of the payment method</param>
		/// <param name="description">The description of the payment method</param>
		/// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
		public override IPaymentGatewayMethod CreatePaymentMethod(IGatewayResource gatewayResource, string name, string description)
		{
			// assert gateway resource is still available
			var available = this.ListResourcesOffered().FirstOrDefault(x => x.ServiceCode == gatewayResource.ServiceCode);
			if (available == null) throw new InvalidOperationException("GatewayResource has already been assigned");

			var attempt = this.GatewayProviderService.CreatePaymentMethodWithKey(this.GatewayProviderSettings.Key, name, description, available.ServiceCode);


			if (attempt.Success)
			{
				this.PaymentMethods = null;

				return new PayPalPaymentGatewayMethod(this.GatewayProviderService, attempt.Result, this.GatewayProviderSettings.ExtendedData);
			}

			LogHelper.Error<PayPalPaymentGatewayProvider>(string.Format("Failed to create a payment method name: {0}, description {1}, paymentCode {2}", name, description, available.ServiceCode), attempt.Exception);

			throw attempt.Exception;
		}

		/// <summary>
		/// Gets a <see cref="IPaymentGatewayMethod"/> by it's unique 'key'
		/// </summary>
		/// <param name="paymentMethodKey">The key of the <see cref="IPaymentMethod"/></param>
		/// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
		public override IPaymentGatewayMethod GetPaymentGatewayMethodByKey(Guid paymentMethodKey)
		{
			var paymentMethod = this.PaymentMethods.FirstOrDefault(x => x.Key == paymentMethodKey);

			if (paymentMethod == null) throw new NullReferenceException("PaymentMethod not found");

			return new PayPalPaymentGatewayMethod(this.GatewayProviderService, paymentMethod, this.GatewayProviderSettings.ExtendedData);
		}

		/// <summary>
		/// Gets a <see cref="IPaymentGatewayMethod"/> by it's payment code
		/// </summary>
		/// <param name="paymentCode">The payment code of the <see cref="IPaymentGatewayMethod"/></param>
		/// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
		public override IPaymentGatewayMethod GetPaymentGatewayMethodByPaymentCode(string paymentCode)
		{
			var paymentMethod = this.PaymentMethods.FirstOrDefault(x => x.PaymentCode == paymentCode);

			if (paymentMethod == null) throw new NullReferenceException("PaymentMethod not found");

			return new PayPalPaymentGatewayMethod(this.GatewayProviderService, paymentMethod, this.GatewayProviderSettings.ExtendedData);
		}
    }
}
