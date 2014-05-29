using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Gateways.Payment;

namespace Merchello.Core.Models.MonitorModels
{
    /// <summary>
    /// Extension methods for Notification Monitor Models
    /// </summary>
    internal static class MonitorModelExtensions
    {
        internal static IPaymentResultMonitorModel ToOrderConfirmationNotification(this IPaymentResult paymentResult)
        {
            return paymentResult.ToOrderConfirmationNotification(new string[] {});
        }

        internal static IPaymentResultMonitorModel ToOrderConfirmationNotification(this IPaymentResult paymentResult, IEnumerable<string> contacts)
        {
            return new PaymentResultMonitorModel()
                {
                    PaymentSuccess = paymentResult.Payment.Success,
                    Payment = paymentResult.Payment.Success ? paymentResult.Payment.Result : null,
                    Invoice = paymentResult.Invoice,
                    Contacts = contacts.ToArray()
                };
        }
    }
}