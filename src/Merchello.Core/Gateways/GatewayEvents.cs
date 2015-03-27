namespace Merchello.Core.Gateways
{
    using System;
    using System.Linq;

    using Merchello.Core.Events;
    using Merchello.Core.Gateways.Notification.Monitors;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Observation;

    using Models;
    using Services;
    using Shipping;
    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The gateway events.
    /// </summary>
    public class GatewayEvents : ApplicationEventHandler
    {
        /// <summary>
        /// The Umbraco Application Started handler.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        /// <remarks>
        /// Merchello is boot strapped in Application Starting so the GatewayProviderResolver should be good to go at this point.
        /// </remarks>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,  ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<GatewayEvents>("Initializing Merchello Warehouse Catalog binding events");

            WarehouseCatalogService.Deleted += WarehouseCatalogServiceDeleted;

            PaymentGatewayMethodBase.AuthorizeAttempted += PaymentGatewayMethodBaseOnAuthorizeAttempted;

            PaymentGatewayMethodBase.AuthorizeCaptureAttempted += PaymentGatewayMethodBaseOnAuthorizeCaptureAttempted;

            PaymentGatewayMethodBase.CaptureAttempted += PaymentGatewayMethodBaseOnCaptureAttempted;

            NotificationMessageService.Saved += NotificationMessageServiceOnSaved;
        }

        /// <summary>
        /// Clears messages from NotificationMonitors cache.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="saveEventArgs">
        /// The save event args.
        /// </param>
        private static void NotificationMessageServiceOnSaved(INotificationMessageService sender, SaveEventArgs<INotificationMessage> saveEventArgs)
        {
            var resolver = MonitorResolver.HasCurrent ? MonitorResolver.Current : null;
            if (resolver == null) return;

            var monitors = resolver.GetAllMonitors();

            foreach (var implements in monitors.OfType<INotificationMonitorBase>())
            {
                implements.RebuildCache();
            }
        }

        /// <summary>
        /// Creates an order if approved
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        private static void CreateOrder(IPaymentResult result)
        {
            if (!result.Payment.Success || !result.ApproveOrderCreation) return;

            // order
            var order = result.Invoice.PrepareOrder(MerchelloContext.Current);

            MerchelloContext.Current.Services.OrderService.Save(order);
        }

        /// <summary>
        /// Handles the capture attempted event
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The payment attempt event args.
        /// </param>
        private void PaymentGatewayMethodBaseOnCaptureAttempted(PaymentGatewayMethodBase sender, PaymentAttemptEventArgs<IPaymentResult> e)
        {
            CreateOrder(e.Entity);
        }

        /// <summary>
        /// Handles the authorize capture attempted event
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The payment attempt event args.
        /// </param>
        private void PaymentGatewayMethodBaseOnAuthorizeCaptureAttempted(PaymentGatewayMethodBase sender, PaymentAttemptEventArgs<IPaymentResult> e)
        {
            CreateOrder(e.Entity);
        }

        /// <summary>
        /// Handles the authorize attempted event
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The payment attempt event args.
        /// </param>
        private void PaymentGatewayMethodBaseOnAuthorizeAttempted(PaymentGatewayMethodBase sender, PaymentAttemptEventArgs<IPaymentResult> e)
        {
            CreateOrder(e.Entity);
        }
        

        /// <summary>
        /// The warehouse catalog service deleted.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="deleteEventArgs">
        /// The delete event args.
        /// </param>
        /// <remarks>
        /// The repository will delete the ship countries and the ship methods but we need to clean up any stored in memory
        /// </remarks>
        private void WarehouseCatalogServiceDeleted(IWarehouseCatalogService sender, DeleteEventArgs<IWarehouseCatalog> deleteEventArgs)
        {
            var providers = GatewayProviderResolver.Current.GetActivatedProviders<ShippingGatewayProviderBase>();

            foreach (var provider in providers)
            {
                ((ShippingGatewayProviderBase)provider).ResetShipMethods();
            }
        }
    }
}