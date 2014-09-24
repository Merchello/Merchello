namespace Merchello.Plugin.Payments.Braintree
{
    using System;
    using System.Linq;

    using Braintree;

    using Core.Services;

    using global::Braintree;

    using Merchello.Core.Models;
    using Merchello.Plugin.Payments.Braintree.Models;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;


    /// <summary>
    /// Handles Umbraco application events.
    /// </summary>
    public class UmbracoApplicationEvents : ApplicationEventHandler
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

            LogHelper.Info<UmbracoApplicationEvents>("Initializing BrainTree Payment provider events");

            GatewayProviderService.Saving += GatewayProviderServiceOnSaving;

            AutoMapperMappings.CreateMappings();
        }

        /// <summary>
        /// The gateway provider service on saving.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="saveEventArgs">
        /// The save event args.
        /// </param>
        private void GatewayProviderServiceOnSaving(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> saveEventArgs)
        {
            var key = new Guid("D143E0F6-98BB-4E0A-8B8C-CE9AD91B0969");
            var provider = saveEventArgs.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);

            if (provider == null) return;

            provider.ExtendedData.SaveProviderSettings(new BraintreeProviderSettings());
        }
    }
}