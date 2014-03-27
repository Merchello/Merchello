using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace Merchello.Web.GatewayProviders.Payment.Cash
{
    /// <summary>
    /// Represents a CashPaymentGatewayProvider
    /// </summary>
    [GatewayProviderActivation("B2612C3D-8BF0-411C-8C56-32E7495AE79C", "Cash Payment Provider", "Cash Payment Provider")]
    public class CashPaymentGatewayProvider : PaymentGatewayProviderBase, ICashPaymentGatewayProvider
    {
        #region AvailableResources

        private static readonly IEnumerable<IGatewayResource> AvailableResources = new List<IGatewayResource>()
        {
            new GatewayResource("Cash", "Cash")
        };

        #endregion

        public CashPaymentGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }

        /// <summary>
        /// Creates a <see cref="IPaymentGatewayMethod"/>
        /// </summary>
        /// <param name="name">The name of the payment method</param>
        /// <param name="description">The description of the payment method</param>
        /// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
        public override IPaymentGatewayMethod CreatePaymentMethod(string name, string description)
        {
            var paymentCode = AvailableResources.First().ServiceCode + "-" + Guid.NewGuid();

            var attempt = GatewayProviderService.CreatePaymentMethodWithKey(GatewayProvider.Key, name, description, paymentCode);

            if (attempt.Success)
            {
                PaymentMethods = null;

                return new CashPaymentGatewayMethod(GatewayProviderService, attempt.Result);
            }
            
            LogHelper.Error<CashPaymentGatewayProvider>(string.Format("Failed to create a payment method name: {0}, description {1}, paymentCode {2}", name, description, paymentCode), attempt.Exception);

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

            if(paymentMethod == null) throw new NullReferenceException("PaymentMethod not found");

            return new CashPaymentGatewayMethod(GatewayProviderService, paymentMethod);
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

            return new CashPaymentGatewayMethod(GatewayProviderService, paymentMethod);
        }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="IGatewayResource"/></returns>
        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            return AvailableResources;
        }

        ///// <summary>
        ///// Gets the name of the provider
        ///// </summary>
        //public override string Name
        //{
        //    get { return "Cash Payment Provider"; }
        //}

        ///// <summary>
        ///// Gets the unique 'key' of the provider
        ///// </summary>
        //public override Guid Key
        //{
        //    get { return new Guid("395D4A61-3A2A-4B4F-AC65-949C33D8611F"); }
        //}

    }
}