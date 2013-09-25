using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Gateway;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Core.Strategies;

namespace Merchello.Tests.Base.Gateway
{
    public class MockGatewayContext
    {
        private readonly IRegisteredGatewayProviderService _registeredGatewayProviderService;
        private readonly IPaymentGatewayProvider _paymentGatewayProvider;

        public MockGatewayContext(IRegisteredGatewayProviderService registeredGatewayProviderService)
            : this(registeredGatewayProviderService, new PaymentGatewayProvider())
        { }

        public MockGatewayContext(IRegisteredGatewayProviderService registeredGatewayProviderService,
                                  IPaymentGatewayProvider paymentGatewayProvider)
        {
            _registeredGatewayProviderService = registeredGatewayProviderService;
            _paymentGatewayProvider = paymentGatewayProvider;
        }

        public IPaymentGatewayProvider Instantiate(Guid providerKey)
        {
            return null;
        }

        // TODO : TryGetInstance as an Attempt
        private IGatewayProvider GetInstance(Guid providerKey)
        {
            var registered = _registeredGatewayProviderService.GetByKey(providerKey);
            var registeredType = Type.GetType(registered.TypeFullName);

            if(registeredType == null) throw new InvalidOperationException("registeredType");

            var ctrArgs = new[] {typeof (IGatewayProvider)};

            IGatewayProvider providerValue;
            switch (registered.GatewayProviderType)
            {
                case GatewayProviderType.Payment:
                    providerValue = _paymentGatewayProvider;
                    break;
                default:
                    throw new NotImplementedException("Custom providers are not yet implemented");
            }

            var ctrValue = new object[] {providerValue};

            var constructor = registeredType.GetConstructor(ctrArgs);
            return constructor.Invoke(ctrValue) as IGatewayProvider;

        }



        // this will require a certain known set of methods between all gateway provider types (or one method)
        private IGatewayProvider GetInstance(Guid providerKey, IGatewayProviderStrategy gatewayProviderStrategy)
        {
            return null;
        }
    }

    public class PaymentGatewayProvider : IPaymentGatewayProvider
    {
    }

    public class PaymentGatewayStrategy
    {
    }

    public interface IPaymentGateway
    {
    }

    public abstract class PaymentGatewayBase : IGatewayProvider
    {
        protected PaymentGatewayBase(IPaymentGatewayStrategy paymentGatewayStrategy)
        {
            
        }
    }

    public interface IPaymentGatewayStrategy
    {
    }
}
