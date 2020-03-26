namespace Merchello.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.Models;
    using Merchello.Providers.Resolvers;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Web;
    using Umbraco.Web.UI.JavaScript;

    /// <summary>
    /// Handles Umbraco application events.
    /// </summary>
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
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<UmbracoApplicationEvents>("Initializing BrainTree Payment provider events");

            GatewayProviderService.Saving += GatewayProviderServiceOnSaving;
        }

        /// <summary>
        /// The gateway provider service on saving.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The save event args.
        /// </param>
        private static void GatewayProviderServiceOnSaving(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> e)
        {
            foreach (var record in e.SavedEntities.ToArray())
            {
                if (record == null || record.HasIdentity) continue;

                // we have to include inactive providers here since this is the process of activating the provider
                var provider = MerchelloContext.Current.Gateways.Payment.GetProviderByKey(record.Key, false);
                if (provider == null) continue;
                
                var attempt = ProviderSettingsResolver.Current.ResolveByType(provider.GetType());

                if (attempt.Success)
                {
                    record.SaveProviderSettings(provider, attempt.Result);
                }
            }
        }
    }
}