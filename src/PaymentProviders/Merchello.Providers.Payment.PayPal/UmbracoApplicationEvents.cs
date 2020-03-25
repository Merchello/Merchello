namespace Merchello.Providers.Payment.PayPal
{
    using Merchello.Core;
    using Merchello.Core.Events;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Observation;
    using Merchello.Providers.Payment.PayPal.Controllers;

    using Umbraco.Core;

    public class UmbracoApplicationEvents : ApplicationEventHandler
    {
        /// <summary>
        /// Handles the Umbraco Application Started event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        protected override void ApplicationStarted(
            UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            PayPalExpressController.Processed += PayPalExpressControllerProcessed;
        }

        /// <summary>
        /// Handles the PayPalExpressCheckoutController processed event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void PayPalExpressControllerProcessed(PayPalExpressController sender, PaymentAttemptEventArgs<IPaymentResult> e)
        {
            var attempt = e.Entity;
            if (attempt.Payment.Success)
            {
                var email = attempt.Invoice.BillToEmail;
                Notification.Trigger("OrderConfirmation", attempt, new[] { email }, Topic.Notifications);
            }
        }
    }
}