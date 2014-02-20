using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Defines the GatewayContext
    /// </summary>
    public interface IGatewayContext
    {
        /// <summary>
        /// Gets the <see cref="IShippingContext"/>
        /// </summary>
        IShippingContext Shipping { get; }

        /// <summary>
        /// Gets the <see cref="ITaxationContext"/>
        /// </summary>
        ITaxationContext Taxation { get; }

        /// <summary>
        /// Gets the <see cref="IPaymentContext"/>
        /// </summary>
        IPaymentContext Payment { get; }
    }
}