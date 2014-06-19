namespace Merchello.Core.Gateways
{
    using Notification;
    using Payment;
    using Shipping;
    using Taxation;

    /// <summary>
    /// Defines the GatewayContext
    /// </summary>
    public interface IGatewayContext
    {
        /// <summary>
        /// Gets the <see cref="IPaymentContext"/>
        /// </summary>
        IPaymentContext Payment { get; }

        /// <summary>
        /// Gets the <see cref="INotificationContext"/>
        /// </summary>
        INotificationContext Notification { get; }

        /// <summary>
        /// Gets the <see cref="IShippingContext"/>
        /// </summary>
        IShippingContext Shipping { get; }

        /// <summary>
        /// Gets the <see cref="ITaxationContext"/>
        /// </summary>
        ITaxationContext Taxation { get; }
    }
}