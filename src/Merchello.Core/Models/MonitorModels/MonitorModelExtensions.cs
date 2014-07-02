using Merchello.Core.Gateways.Notification.Triggering;

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

        /// <summary>
        /// To the order shipped notification.
        /// </summary>
        /// <param name="paymentResult">The payment result.</param>
        /// <returns></returns>
        public static IPaymentResultMonitorModel ToOrderShippedNotification(this IPaymentResult paymentResult)
        {
            return paymentResult.ToOrderConfirmationNotification(new string[] { });
        }

        /// <summary>
        /// To the order shipped notification.
        /// </summary>
        /// <param name="paymentResult">The payment result.</param>
        /// <param name="contacts">The contacts.</param>
        /// <returns></returns>
        public static IShipmentResult ToOrderShippedNotification(this IShipmentResult shipmentResult, IEnumerable<string> contacts)
        {
            return new ShippingResultNotifyModel()
            {
                Shipment = shipmentResult.Shipment,
                Invoice = shipmentResult.Invoice,
                Contacts = contacts.ToArray()
            };
        }       
    }
}