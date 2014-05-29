using System;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using Merchello.Core.Observation;
using Merchello.Core.Services;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Represents the GatewayContext.  Provides access to <see cref="IGatewayProviderSettings"/>s
    /// </summary>
    internal class GatewayContext : IGatewayContext
    {
        private Lazy<INotificationContext> _notification;
        private Lazy<IPaymentContext> _payment;        
        private Lazy<IShippingContext> _shipping;
        private Lazy<ITaxationContext> _taxation;
        private readonly IGatewayProviderService _gatewayProviderService;
        private readonly IGatewayProviderResolver _resolver;

        internal GatewayContext(IServiceContext serviceContext, IGatewayProviderResolver resolver)
        {
            Mandate.ParameterNotNull(serviceContext, "serviceContext");
            Mandate.ParameterNotNull(resolver, "resolver");

            _gatewayProviderService = serviceContext.GatewayProviderService;
            _resolver = resolver;


            BuildGatewayContext(serviceContext.GatewayProviderService, serviceContext.StoreSettingService);
        }

        private void BuildGatewayContext(IGatewayProviderService gatewayProviderService, IStoreSettingService storeSettingService)
        {
            if(_notification == null)
                _notification = new Lazy<INotificationContext>(() => new NotificationContext(gatewayProviderService, _resolver));

            if (_payment == null)
                _payment = new Lazy<IPaymentContext>(() => new PaymentContext(gatewayProviderService, _resolver));

            if(_shipping == null) 
                _shipping = new Lazy<IShippingContext>(() => new ShippingContext(gatewayProviderService, storeSettingService, _resolver));
           
            if(_taxation == null)
                _taxation = new Lazy<ITaxationContext>(() => new TaxationContext(gatewayProviderService, _resolver));

        }

        /// <summary>
        /// Exposes the <see cref="IPaymentContext"/>
        /// </summary>
        public IPaymentContext Payment
        {
            get
            {
                if (_payment == null) throw new InvalidOperationException("The PaymentContext is not set in the GatewayContext");

                return _payment.Value;
            }
        }

        /// <summary>
        /// Exposes the <see cref="INotificationContext"/>
        /// </summary>
        public INotificationContext Notification
        {
            get
            {
                if(_notification == null) throw new InvalidOperationException("The NotificationContext is not set in the GatewayContext");

                return _notification.Value;
            }
        }

        /// <summary>
        /// Exposes the <see cref="IShippingContext"/>
        /// </summary>
        public IShippingContext Shipping
        {
            get
            {
                if(_shipping == null) throw new InvalidOperationException("The ShippingContext is not set in the GatewayContext");

                return _shipping.Value;
            }

        }

        /// <summary>
        /// Exposes the <see cref="ITaxationContext"/>
        /// </summary>
        public ITaxationContext Taxation
        {
            get
            {
                if (_taxation == null) throw new InvalidOperationException("The TaxationContext is not set in the GatewayContext");

                return _taxation.Value;
            } 
        }


        /// <summary>
        /// For testing
        /// </summary>
        internal void DeactivateProvider(GatewayProviderBase provider)
        {
            if (!provider.Activated) return;
            _gatewayProviderService.Delete(provider.GatewayProviderSettings);
            GatewayProviderResolver.Current.RefreshCache();
        }
    }
}