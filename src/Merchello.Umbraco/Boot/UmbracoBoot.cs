namespace Merchello.Umbraco.Boot
{
    using System;
    using System.Reflection;

    using log4net;

    using Merchello.Core.Boot;

    using global::Umbraco.Core;

    /// <summary>
    /// Handles Umbraco's Initialized event to start Merchello's bootstrap process.
    /// </summary>
    /// <remarks>
    /// FYI: This is a partial class so we can nest actual event handler registrations in a more organized fashion
    /// </remarks> 
    public partial class UmbracoBoot : IApplicationEventHandler
    {
        /// <summary>
        /// A logger to log startup
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <inheritdoc/>
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        /// <inheritdoc/>
        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }
    
        /// <inheritdoc/>
        /// <remarks>
        /// Merchello starts it's boot sequence after Umbraco has completed
        /// </remarks>
        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            BootManagerBase.MerchelloStarted += this.OnMerchelloStarted;

            try
            {
                // Initialize Merchello
                Log.Info("Attempting to initialize Merchello Umbraco Package");

                // Build the settings and adapt Umbraco singleton instances
                var settings = GetBootSettings(applicationContext);

                MerchelloBootstrapper.Init(new UmbracoBootManager(settings));
                
                Log.Info("Initialization of Merchello Umbraco Package complete");
            }
            catch (Exception ex)
            {
                Log.Error("Initialization of Merchello failed", ex);
            }
        }

        /// <summary>
        /// Handles the <see cref="BootManagerBase"/> Started event.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="BootManagerBase"/>.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/>.
        /// </param>>
        private void OnMerchelloStarted(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}