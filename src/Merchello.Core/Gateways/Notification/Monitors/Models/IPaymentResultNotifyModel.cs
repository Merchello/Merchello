using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Notification.Monitors.Models
{
    public interface IPaymentResultNotifyModel
    {
        /// <summary>
        /// Gets/sets the <see cref="IPayment"/>
        /// </summary>
        IPayment Payment { get; set; }

        /// <summary>
        /// Get/sets the <see cref="IInvoice"/>
        /// </summary>
        IInvoice Invoice { get; set; }

        bool PaymentSuccess { get; set; }

        bool ApproveOrderCreation { get; set; }
    }
}