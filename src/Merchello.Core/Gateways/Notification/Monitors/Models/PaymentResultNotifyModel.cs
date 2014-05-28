using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Notification.Monitors.Models
{
    internal class PaymentResultNotifyModel : IPaymentResultNotifyModel
    {
        public string[] Contacts { get; set; }
        public IPayment Payment { get; set; }
        public IInvoice Invoice { get; set; }
        public bool PaymentSuccess { get; set; }
        public bool ApproveOrderCreation { get; set; }
    }
}