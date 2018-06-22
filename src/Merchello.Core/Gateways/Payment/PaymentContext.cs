

namespace Merchello.Core.Gateways.Payment
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Services;

    /// <summary>
    /// Represents the Payment Context
    /// </summary>
    internal class PaymentContext : GatewayProviderTypedContextBase<PaymentGatewayProviderBase>, IPaymentContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentContext"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="resolver">
        /// The resolver.
        /// </param>
        public PaymentContext(IGatewayProviderService gatewayProviderService, IGatewayProviderResolver resolver)
            : base(gatewayProviderService, resolver)
        {
        }

        /// <summary>
        /// Returns an instance of an 'active' GatewayProvider associated with a GatewayMethod based given the unique Key (GUID) of the GatewayMethod
        /// </summary>
        /// <param name="gatewayMethodKey">The unique key (GUID) of the <see cref="IGatewayMethod"/></param>
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
            var paymentProviders = GatewayProviderResolver.GetActivatedProviders<PaymentGatewayProviderBase>() as List<PaymentGatewayProviderBase>;
            
            var methods = new List<IPaymentGatewayMethod>();
            if (paymentProviders == null) return methods;

            // Get the provders
            foreach (var provider in paymentProviders)
            {
                var paymentMethods = provider.PaymentMethods.ToList();
                methods.AddRange(paymentMethods.Select(x => provider.GetPaymentGatewayMethodByKey(x.Key)));
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