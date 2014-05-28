using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Gateways.Payment;

namespace Merchello.Core.Gateways.Notification.Monitors.Models
{
    /// <summary>
    /// Extension methods for Notification Monitor Models
    /// </summary>
    internal static class MonitorModelExtensions
    {
        internal static IPaymentResultNotifyModel ToOrderConfirmationNotification(this IPaymentResult paymentResult)
        {
            return paymentResult.ToOrderConfirmationNotification(new string[] {});
        }

        internal static IPaymentResultNotifyModel ToOrderConfirmationNotification(this IPaymentResult paymentResult, IEnumerable<string> contacts)
        {
            return new PaymentResultNotifyModel()
                {
                    PaymentSuccess = paymentResult.Payment.Success,
                    Payment = paymentResult.Payment.Success ? paymentResult.Payment.Result : null,
                    Invoice = paymentResult.Invoice,
                    Contacts = contacts.ToArray()
                };
        }
    }
}