using Merchello.Core.Gateways.Payment;

namespace Merchello.Core.Gateways.Notification.Monitors.Models
{
    internal static class MonitorModelExtensions
    {
         internal static IOrderConfirmationModel ToOrderConfirmationNotification(this IPaymentResult paymentResult)
         {
             return new OrderConfirmationModel()
                 {
                     PaymentSuccess = paymentResult.Payment.Success,
                     Payment = paymentResult.Payment.Success ? paymentResult.Payment.Result : null,
                     Invoice = paymentResult.Invoice,
                     
                 };
         }
    }
}