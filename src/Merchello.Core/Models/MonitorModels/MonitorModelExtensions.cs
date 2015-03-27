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
        /// <summary>
        /// The to order confirmation notification.
        /// </summary>
        /// <param name="paymentResult">
        /// The payment result.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResultMonitorModel"/>.
        /// </returns>
        public static IPaymentResultMonitorModel ToOrderConfirmationNotification(this IPaymentResult paymentResult)
        {
            return paymentResult.ToOrderConfirmationNotification(new string[] { });
        }

        /// <summary>
        /// The to order confirmation notification.
        /// </summary>
        /// <param name="paymentResult">
        /// The payment result.
        /// </param>
        /// <param name="contacts">
        /// The contacts.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResultMonitorModel"/>.
        /// </returns>
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
        /// <param name="paymentResult">
        /// The payment result.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResultMonitorModel"/>.
        /// </returns>
        public static IPaymentResultMonitorModel ToOrderShippedNotification(this IPaymentResult paymentResult)
        {
            return paymentResult.ToOrderConfirmationNotification(new string[] { });
        }

        /// <summary>
        /// To the order shipped notification.
        /// </summary>
        /// <param name="shipmentResult">
        /// The shipment Result.
        /// </param>
        /// <param name="contacts">
        /// The contacts.
        /// </param>
        public static IShipmentResultNotifyModel ToOrderShippedNotification(this IShipmentResultNotifyModel shipmentResult, IEnumerable<string> contacts)
        {
            return new ShipmentResultNotifyModel()
            {
                Shipment = shipmentResult.Shipment,
                Invoice = shipmentResult.Invoice,
                Contacts = contacts.ToArray()
            };
        }       
    }
}