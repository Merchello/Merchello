using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Payment
{
    public abstract class PaymentGatewayProviderBase  : GatewayProviderBase, IPaymentGatewayProvider
    {
        protected PaymentGatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }


        public IPaymentMethod CreatePaymentMethod(string name, string description, string paymentCode)
        {
            throw new NotImplementedException();
        }

        public void SavePaymentMethod(IPaymentMethod paymentMethod)
        {
            throw new NotImplementedException();
        }

        public void DeletePaymentMethod(IPaymentMethod paymentMethod)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPaymentMethod> PaymentMethods { get; private set; }
    }
}