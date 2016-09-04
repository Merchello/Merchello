namespace Merchello.Web.Umbraco
{
    using System;

    using global::Umbraco.Core;

    using Merchello.Core;

    using umbraco.BusinessLogic;

    /// <summary>
    /// Handles Umbraco's Initialized event to start Merchello's bootstrap process.
    /// </summary>
    public class Boot : IApplicationEventHandler
    {

        ///// <summary>
        ///// A logger to log startup
        ///// </summary>
        //private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <inheritdoc/>
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            BootManagerBase.MerchelloStarted += BootManagerBaseOnMerchelloStarted;

            try
            {
                // Initialize Merchello
                //Log.Info("Attempting to initialize Merchello");
                MerchelloBootstrapper.Init(new WebBootManager());
                //Log.Info("Initialization of Merchello complete");
            }
            catch (Exception ex)
            {
               // Log.Error("Initialization of Merchello failed", ex);
            }

        }
        /// <inheritdoc/>
        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }
    
        /// <inheritdoc/>
        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        private void BootManagerBaseOnMerchelloStarted(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}