namespace Merchello.Web
{
    using System;
    using System.Reflection;
    using Core;
    using log4net;
    using Umbraco.Core;

    /// <summary>
    /// Listens for the Umbraco Application "Started" event and initiates the Merchello startup
    /// </summary>
    public class UmbracoApplicationEventListener : ApplicationEventHandler
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);

            // Initialize Merchello
            Log.Info("Attempting to initialize Merchello");
            try
            {
                MerchelloBootstrapper.Init(new WebBootManager());
                Log.Info("Initialization of Merchello complete");                
            }
            catch (Exception ex)
            {
                Log.Error("Initialization of Merchello failed", ex);
            }
        }        
    }
}
