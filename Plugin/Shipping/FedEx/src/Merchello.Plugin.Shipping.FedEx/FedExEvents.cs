using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Shipping.FedEx.Models;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;

namespace Merchello.Plugin.Shipping.FedEx
{
    public class FedExEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
                                              ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<FedExEvents>("Initializing FedEx Shipping provider registration binding events");


            GatewayProviderService.Saving += delegate(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> args)
            {
                var key = new Guid("646d3ea7-3b31-45c1-9488-7c0449a564a6");
                var provider = args.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);
                if (provider == null) return;

                provider.ExtendedData.SaveProcessorSettings(new FedExProcessorSettings());

            };
        }
    }
}
