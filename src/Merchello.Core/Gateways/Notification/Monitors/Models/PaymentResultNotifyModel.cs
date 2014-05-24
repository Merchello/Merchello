using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Notification.Monitors.Models
{
    internal class OrderConfirmationModel : IOrderConfirmationModel
    {
        public IPayment Payment { get; set; }
        public IInvoice Invoice { get; set; }
        public bool PaymentSuccess { get; set; }
    }
}