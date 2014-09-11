namespace Merchello.Plugin.Payments.Chase
{
    using System;
    using System.Linq;
    using Core.Models;
    using Core.Services;
    using Models;
    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Authorize net package events.
    /// </summary>
    public class ChaseEvents : ApplicationEventHandler
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

            LogHelper.Info<ChaseEvents>("Initializing Chase provider registration binding events");
            
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
            var key = new Guid("D584F356-454B-4D14-BE44-13D9D25D6A74");
            var provider = args.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);

            if (provider == null) return;

            provider.ExtendedData.SaveProcessorSettings(new ChaseProcessorSettings());
        }
    }
}