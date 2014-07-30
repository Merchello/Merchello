using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.Stripe.Models;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;

namespace Merchello.Plugin.Payments.Stripe
{
    class StripeApplicationEventListener
    {
        public class StripeEvents : ApplicationEventHandler
        {
            protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
                                                       ApplicationContext applicationContext)
            {
                base.ApplicationStarted(umbracoApplication, applicationContext);

                LogHelper.Info<StripeEvents>("Initializing Stripe provider registration binding events");

                GatewayProviderService.Saving += GatewayProviderServiceOnSaving;
            }

            private static void GatewayProviderServiceOnSaving(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> args)
            {
                var key = new Guid("15C87B6F-7987-49D9-8444-A2B4406941A8");
                var provider = args.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);
                if (provider == null) return;

                provider.ExtendedData.SaveProcessorSettings(new StripeProcessorSettings());
            }

        }
    }
}
