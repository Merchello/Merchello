using System;
using Merchello.Core;
using umbraco.BusinessLogic;
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
            
            UmbracoApplicationBase.ApplicationStarted += delegate(object sender, EventArgs args)
            {
                var wbm = new WebBootManager();
                    wbm.Initialize()
                    .Startup(merchContext => wbm.OnMerchelloStarted(this, new EventArgs()))
                    .Complete(merchContext => wbm.OnMerchelloStarted(this, new EventArgs()));
    

            };
            
        }
    }
}
