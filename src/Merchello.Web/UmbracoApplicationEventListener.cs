using Merchello.Core;
using System.IO;
using Umbraco.Core;

namespace Merchello.Web
{
    /// <summary>
    /// Listens for the Umbraco Application "Started" event and initiates the Merchello startup
    /// </summary>
    public class UmbracoApplicationEventListener : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            string merchelloConfigFileLocation = @"~\App_Plugins\Merchello\Config\merchello.config";
            if (File.Exists(merchelloConfigFileLocation))
            {
                MerchelloBootstrapper.Init(new WebBootManager());
            }
        }
    }
}
