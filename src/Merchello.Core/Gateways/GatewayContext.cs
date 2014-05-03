using System;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Represents the GatewayContext.  Provides access to <see cref="IGatewayProvider"/>s
    /// </summary>
    internal class GatewayContext : IGatewayContext
    {
        private readonly IPaymentContext _payment;
        private readonly INotificationContext _notification;
        private readonly IShippingContext _shipping;
        private readonly ITaxationContext _taxation;
        

        internal GatewayContext(IPaymentContext payment, INotificationContext notification, IShippingContext shipping, ITaxationContext taxation)
        {
            Mandate.ParameterNotNull(payment, "payment");
            Mandate.ParameterNotNull(notification, "notification");
            Mandate.ParameterNotNull(shipping, "shipping");
            Mandate.ParameterNotNull(taxation, "taxation");

            _payment = payment;
            _notification = notification;
            _shipping = shipping;
            _taxation = taxation;
            
        }


        /// <summary>
        /// Exposes the <see cref="IPaymentContext"/>
        /// </summary>
        public IPaymentContext Payment
        {
            get
            {
                if (_payment == null) throw new InvalidOperationException("The PaymentContext is not set in the GatewayContext");

                return _payment;
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

                return _notification;
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

                return _shipping;
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

                return _taxation;
            } 
        }


    }
}