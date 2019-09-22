using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Events;

namespace Merchello.Core
{
    /// <summary>
    ///     Handles the Umbraco Application "Starting" and "Started" event and initiates the Merchello startup
    /// </summary>
    public class UmbracoApplicationEventHandler : ApplicationEventHandler
    {
        /// <summary>
        ///     The Umbraco Application Starting event.
        /// </summary>
        /// <param name="umbracoApplication">
        ///     The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        ///     The application context.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            StoreSettingService.Saved += StoreSettingServiceSaved;
        }


        /// <summary>
        ///     Resets the store currency.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The save event args.
        /// </param>
        private void StoreSettingServiceSaved(
            IStoreSettingService sender,
            SaveEventArgs<IStoreSetting> e)
        {
            CurrencyContext.Current.ResetCurrency();
        }
    }
}