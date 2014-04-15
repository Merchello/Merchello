using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Payment
{
    /// <summary>
    /// Represents an abstract PaymentGatewayProvider
    /// </summary>
    public abstract class PaymentGatewayProviderBase  : GatewayProviderBase, IPaymentGatewayProvider
    {
        protected PaymentGatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }

        /// <summary>
        /// Creates a <see cref="IPaymentGatewayMethod"/>
        /// </summary>
        /// <param name="gatewayResource">The <see cref="IGatewayResource"/> implemented by this method</param>
        /// <param name="name">The name of the payment method</param>
        /// <param name="description">The description of the payment method</param>
        /// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
        public abstract IPaymentGatewayMethod CreatePaymentMethod(IGatewayResource gatewayResource, string name, string description);

        /// <summary>
        /// Saves a <see cref="IPaymentGatewayMethod"/>
        /// </summary>
        /// <param name="method">The <see cref="IPaymentGatewayMethod"/> to be saved</param>
        public virtual void SavePaymentMethod(IPaymentGatewayMethod method)
        {
            GatewayProviderService.Save(method.PaymentMethod);

            PaymentMethods = null;
        }

        /// <summary>
        /// Deletes a <see cref="IPaymentGatewayMethod"/>
        /// </summary>
        /// <param name="method">The <see cref="IPaymentGatewayMethod"/> to delete</param>
        public virtual void DeletePaymentMethod(IPaymentGatewayMethod method)
        {
            GatewayProviderService.Save(method.PaymentMethod);
        }

        /// <summary>
        /// Gets a <see cref="IPaymentGatewayMethod"/> by it's unique 'key'
        /// </summary>
        /// <param name="paymentMethodKey">The key of the <see cref="IPaymentMethod"/></param>
        /// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
        public abstract IPaymentGatewayMethod GetPaymentGatewayMethodByKey(Guid paymentMethodKey);

        /// <summary>
        /// Gets a <see cref="IPaymentGatewayMethod"/> by it's payment code
        /// </summary>
        /// <param name="paymentCode">The payment code of the <see cref="IPaymentGatewayMethod"/></param>
        /// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
        public abstract IPaymentGatewayMethod GetPaymentGatewayMethodByPaymentCode(string paymentCode);
      

        private IEnumerable<IPaymentMethod> _paymentMethods; 

        /// <summary>
        /// Gets a collection of all <see cref="IPaymentMethod"/>s associated with this provider
        /// </summary>
        public IEnumerable<IPaymentMethod> PaymentMethods
        {
            get {
                return _paymentMethods ??
                       (_paymentMethods = GatewayProviderService.GetPaymentMethodsByProviderKey(GatewayProvider.Key));
            }
            protected set { _paymentMethods = value; }
        }
    }
}