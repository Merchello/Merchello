using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Shipping.USPS.Models;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;

namespace Merchello.Plugin.Shipping.USPS
{
    public class UspsEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
                                              ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<UspsEvents>("Initializing USPS Shipping provider registration binding events");


            GatewayProviderService.Saving += delegate(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> args)
            {
                var key = new Guid("94BD59AE-0FA8-45E4-BD9A-3577C964851F");
                var provider = args.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);
                if (provider == null) return;

                provider.ExtendedData.SaveProcessorSettings(new UspsProcessorSettings());

            };
        }
    }
}
