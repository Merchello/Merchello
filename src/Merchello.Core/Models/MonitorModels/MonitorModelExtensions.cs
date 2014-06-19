namespace Merchello.Core.Models.MonitorModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Gateways.Payment;

    /// <summary>
    /// Extension methods for Notification Monitor Models
    /// </summary>
    public static class MonitorModelExtensions
    {
        public static IPaymentResultMonitorModel ToOrderConfirmationNotification(this IPaymentResult paymentResult)
        {
            return paymentResult.ToOrderConfirmationNotification(new string[] { });
        }

        public static IPaymentResultMonitorModel ToOrderConfirmationNotification(this IPaymentResult paymentResult, IEnumerable<string> contacts)
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