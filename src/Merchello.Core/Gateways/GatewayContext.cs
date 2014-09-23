namespace Merchello.Core.Gateways
{
    using System;

    using Merchello.Core.Gateways.Notification;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Gateways.Taxation;
    using Merchello.Core.Models;
    using Merchello.Core.Services;

    /// <summary>
    /// Represents the GatewayContext.  Provides access to <see cref="IGatewayProviderSettings"/>s
    /// </summary>
    internal class GatewayContext : IGatewayContext
    {
        /// <summary>
        /// The gateway provider service.
        /// </summary>
        private readonly IGatewayProviderService _gatewayProviderService;

        /// <summary>
        /// The gateway provider resolver.
        /// </summary>
        private readonly IGatewayProviderResolver _resolver;

        /// <summary>
        /// The notification context.
        /// </summary>
        private Lazy<INotificationContext> _notification;

        /// <summary>
        /// The payment context.
        /// </summary>
        private Lazy<IPaymentContext> _payment;

        /// <summary>
        /// The shipping context.
        /// </summary>
        private Lazy<IShippingContext> _shipping;

        /// <summary>
        /// The taxation context.
        /// </summary>
        private Lazy<ITaxationContext> _taxation;

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayContext"/> class.
        /// </summary>
        /// <param name="serviceContext">
        /// The service context.
        /// </param>
        public GatewayContext(IServiceContext serviceContext)
            : this(serviceContext, GatewayProviderResolver.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayContext"/> class.
        /// </summary>
        /// <param name="serviceContext">
        /// The service context.
        /// </param>
        /// <param name="resolver">
        /// The resolver.
        /// </param>
        internal GatewayContext(IServiceContext serviceContext, IGatewayProviderResolver resolver)
        {
            Mandate.ParameterNotNull(serviceContext, "serviceContext");
            Mandate.ParameterNotNull(resolver, "resolver");

            _gatewayProviderService = serviceContext.GatewayProviderService;
            _resolver = resolver;


            BuildGatewayContext(serviceContext.GatewayProviderService, serviceContext.StoreSettingService);
        }

        /// <summary>
        /// Gets the <see cref="IPaymentContext"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Throws an exception if the <see cref="PaymentContext"/> is null
        /// </exception>
        public IPaymentContext Payment
        {
            get
            {
                if (_payment == null) throw new InvalidOperationException("The PaymentContext is not set in the GatewayContext");

                return _payment.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="INotificationContext"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Throws an exception if the <see cref="NotificationContext"/> is null
        /// </exception>
        public INotificationContext Notification
        {
            get
            {
                if(_notification == null) throw new InvalidOperationException("The NotificationContext is not set in the GatewayContext");

                return _notification.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IShippingContext"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Throws an exception if the <see cref="ShippingContext"/> is null
        /// </exception>
        public IShippingContext Shipping
        {
            get
            {
                if (_shipping == null) throw new InvalidOperationException("The ShippingContext is not set in the GatewayContext");

                return _shipping.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ITaxationContext"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Throws an exception if the <see cref="TaxationContext"/> is null
        /// </exception>
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

        /// <summary>
        /// The build gateway context.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        private void BuildGatewayContext(IGatewayProviderService gatewayProviderService, IStoreSettingService storeSettingService)
        {
            if (_notification == null)
                _notification = new Lazy<INotificationContext>(() => new NotificationContext(gatewayProviderService, _resolver));

            if (_payment == null)
                _payment = new Lazy<IPaymentContext>(() => new PaymentContext(gatewayProviderService, _resolver));

            if (_shipping == null)
                _shipping = new Lazy<IShippingContext>(() => new ShippingContext(gatewayProviderService, storeSettingService, _resolver));

            if (_taxation == null)
                _taxation = new Lazy<ITaxationContext>(() => new TaxationContext(gatewayProviderService, _resolver));
        }
    }
}