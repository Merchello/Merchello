using System;
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
        private readonly IShippingContext _shipping;
        private readonly ITaxationContext _taxation;
        private readonly IPaymentContext _payment;

        internal GatewayContext(IShippingContext shipping, ITaxationContext taxation, IPaymentContext payment)
        {
            Mandate.ParameterNotNull(shipping, "shipping");
            Mandate.ParameterNotNull(taxation, "taxation");
            Mandate.ParameterNotNull(payment, "payment");

            _shipping = shipping;
            _taxation = taxation;
            _payment = payment;
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

        /// <summary>
        /// Exposes teh <see cref="IPaymentContext"/>
        /// </summary>
        public IPaymentContext Payment
        {
            get
            {
                if (_payment == null) throw new InvalidOperationException("The PaymentContext is not set in the GatewayContext");

                return _payment;
            } 
        }

    }
}