using System.Net.Mime;
using log4net;
using Merchello.Core;
using System;
using System.IO;
using System.Reflection;
using Merchello.Core.Services;
using umbraco.interfaces;
using Umbraco.Core;
using Umbraco.Web;

namespace Merchello.Web
{
    /// <summary>
    /// Listens for the Umbraco Application "Started" event and initiates the Merchello startup
    /// </summary>
    public class UmbracoApplicationEventListener : ApplicationEventHandler
    {
        private static readonly ILog Log =
            LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType
            );


        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            // Initialize Merchello
            Log.Info("Attempting to initialize Merchello");
            try
            {
                MerchelloBootstrapper.Init(new WebBootManager());
                Log.Info("Initialization of Merchello complete");
            }
            catch (Exception ex)
            {
                Log.Error("Initialization of Merchello failed - no merchello.config file found", ex);
            }

            // Register 
            
        }
        
    }
}
