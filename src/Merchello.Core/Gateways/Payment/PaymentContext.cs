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
        /// Gets a list of all possible Payment Methods
        /// </summary>
        /// <returns>A collection of <see cref="IPaymentGatewayMethod"/>s</returns>
        public IEnumerable<IPaymentGatewayMethod> GetPaymentGatewayMethods()
        {
            var paymentProviders = GatewayProviderResolver.ActivatedProvidersOf<PaymentGatewayProviderBase>() as IEnumerable<PaymentGatewayProviderBase>;
            
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