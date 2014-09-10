using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.PurchaseOrder.Models;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;

namespace Merchello.Plugin.Payments.PurchaseOrder
{
    /// <summary>
    /// Authorize net package events.
    /// </summary>
    public class PurchaseOrderEvents : ApplicationEventHandler
    {
        /// <summary>
        /// The application started.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<PurchaseOrderEvents>("Initializing PurchaseOrder provider registration binding events");
            
            GatewayProviderService.Saving += GatewayProviderServiceOnSaving;
        }

        /// <summary>
        /// The gateway provider service on saved.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void GatewayProviderServiceOnSaving(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> args)
        {
            var key = new Guid("A5C2BFE1-CC2E-4809-9334-2C215E9E20E0");
            var provider = args.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);

            if (provider == null) return;

            provider.ExtendedData.SaveProcessorSettings(new PurchaseOrderProcessorSettings());
        }
    }
}