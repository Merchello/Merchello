namespace Merchello.Plugin.Payments.AuthorizeNet
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
    public class AuthorizeNetEvents : ApplicationEventHandler
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

            LogHelper.Info<AuthorizeNetEvents>("Initializing AuthorizeNet provider registration binding events");
            
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
            var key = new Guid("C6BF6743-3565-401F-911A-33B68CACB11B");
            var provider = args.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);

            if (provider == null) return;

            provider.ExtendedData.SaveProcessorSettings(new AuthorizeNetProcessorSettings());
        }
    }
}