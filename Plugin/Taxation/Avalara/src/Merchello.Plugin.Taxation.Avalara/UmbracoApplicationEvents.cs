namespace Merchello.Plugin.Taxation.Avalara
{
    using System;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Plugin.Taxation.Avalara.Models;
    using Merchello.Plugin.Taxation.Avalara.Models.Address;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Handles Umbraco application events.
    /// </summary>
    public class UmbracoApplicationEvents : ApplicationEventHandler
    {
        /// <summary>
        /// Handles the Umbraco application started event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The Umbraco application context.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<UmbracoApplicationEvents>("Initializing Avalara AvaTax provider registration binding events");

            GatewayProviderService.Saving += GatewayProviderServiceOnSaving;

            AutoMapper.Mapper.CreateMap<IValidatableAddress, TaxAddress>(); 
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
            var key = new Guid("DBC48C38-0617-44EA-989A-18AAD8D5DE52");
            var provider = saveEventArgs.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);

            if (provider == null) return;

            provider.ExtendedData.SaveProviderSettings(new AvaTaxProviderSettings());
        }
    }
}