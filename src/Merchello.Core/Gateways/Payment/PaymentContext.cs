using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Services;
using Umbraco.Core.Cache;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Gateways.Payment
{
    /// <summary>
    /// Represents the Payment Context
    /// </summary>
    internal class PaymentContext : GatewayProviderTypedContextBase<PaymentGatewayProviderBase>, IPaymentContext
    {
        public PaymentContext(IGatewayProviderService gatewayProviderService, IGatewayProviderResolver resolver) 
            : base(gatewayProviderService, resolver)
        { }

        /// <summary>
        /// Returns an instance of an 'active' GatewayProvider associated with a GatewayMethod based given the unique Key (Guid) of the GatewayMethod
        /// </summary>
        /// <param name="gatewayMethodKey">The unique key (Guid) of the <see cref="IGatewayMethod"/></param>
        /// <returns>An instantiated GatewayProvider</returns>
        public override PaymentGatewayProviderBase GetProviderByMethodKey(Guid gatewayMethodKey)
        {
            return GetAllActivatedProviders()
                .FirstOrDefault(x => ((PaymentGatewayProviderBase)x)
                    .PaymentMethods.Any(y => y.Key == gatewayMethodKey)) as PaymentGatewayProviderBase;
        }

        /// <summary>
        /// Gets a list of all possible Payment Methods
        /// </summary>
        /// <returns>A collection of <see cref="IPaymentGatewayMethod"/>s</returns>
        public IEnumerable<IPaymentGatewayMethod> GetPaymentGatewayMethods()
        {
            var paymentProviders = GatewayProviderResolver.GetActivatedProviders<PaymentGatewayProviderBase>() as IEnumerable<PaymentGatewayProviderBase>;
            
            var methods = new List<IPaymentGatewayMethod>();
            if (paymentProviders == null) return methods;

            foreach (var provider in paymentProviders)
            {
                methods.AddRange(provider.PaymentMethods.Select(x => provider.GetPaymentGatewayMethodByKey(x.Key)));
            }

            return methods;
        }

        /// <summary>
        /// Gets a <see cref="IPaymentGatewayMethod"/> by its unique key
        /// </summary>
        /// <param name="paymentMethodKey">The Key of the <see cref="IPaymentGatewayMethod"/></param>
        /// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
        public IPaymentGatewayMethod GetPaymentGatewayMethodByKey(Guid paymentMethodKey)
        {
            return GetPaymentGatewayMethods().FirstOrDefault(x => x.PaymentMethod.Key.Equals(paymentMethodKey));
        }


    }
}