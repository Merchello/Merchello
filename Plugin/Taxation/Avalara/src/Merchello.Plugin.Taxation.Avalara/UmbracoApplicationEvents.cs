namespace Merchello.Plugin.Taxation.Avalara
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Services;
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

        private void GatewayProviderServiceOnSaving(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> saveEventArgs)
        {
            throw new NotImplementedException();
        }
    }
}