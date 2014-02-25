using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace Merchello.Core.Gateways.Payment.Cash
{
    /// <summary>
    /// Represents a CashPaymentGatewayProvider
    /// </summary>
    public class CashPaymentGatewayProvider : PaymentGatewayProviderBase, ICashPaymentGatewayProvider
    {
        public CashPaymentGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }

        /// <summary>
        /// Creates a <see cref="IPaymentGatewayMethod"/>
        /// </summary>
        /// <param name="name">The name of the payment method</param>
        /// <param name="description">The description of the payment method</param>
        /// <param name="paymentCode">The payment code of the payment method</param>
        /// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
        public override IPaymentGatewayMethod CreatePaymentMethod(string name, string description, string paymentCode)
        {
            var attempt = GatewayProviderService.CreatePaymentMethodWithKey(GatewayProvider.Key, name, description, paymentCode);

            if (!attempt.Success)
            {
                LogHelper.Error<CashPaymentGatewayProvider>(string.Format("Failed to create a payment method name: {0}, description {1}, paymentCode {2}", name, description, paymentCode), attempt.Exception);

                throw attempt.Exception;
            }

            return new CashPaymentMethod(attempt.Result);
        }

        /// <summary>
        /// Gets a <see cref="IPaymentGatewayMethod"/> by it's unique 'key'
        /// </summary>
        /// <param name="paymentMethodKey">The key of the <see cref="IPaymentMethod"/></param>
        /// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
        public override IPaymentGatewayMethod GetByKey(Guid paymentMethodKey)
        {
            var paymentMethod = PaymentMethods.FirstOrDefault(x => x.Key == paymentMethodKey);

            if(paymentMethod == null) throw new NullReferenceException("PaymentMethod not found");

            return new CashPaymentMethod(paymentMethod);
        }


        /// <summary>
        /// Gets the name of the provider
        /// </summary>
        public override string Name
        {
            get { return "Cash Payment Provider"; }
        }

        /// <summary>
        /// Gets the unique 'key' of the provider
        /// </summary>
        public override Guid Key
        {
            get { return new Guid("395D4A61-3A2A-4B4F-AC65-949C33D8611F"); }
        }

    }
}