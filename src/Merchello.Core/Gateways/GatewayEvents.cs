namespace Merchello.Core.Gateways
{
    using System;
    using Shipping;
    using Models;
    using Services;
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