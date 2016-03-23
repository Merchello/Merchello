namespace Merchello.Providers.Payment.PayPal
{
    using System;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Providers.Payment.Models;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    using Constants = Providers.Constants;

    public class PayPalEvents : ApplicationEventHandler
	{
		protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
										   ApplicationContext applicationContext)
		{
			base.ApplicationStarted(umbracoApplication, applicationContext);

			LogHelper.Info<PayPalEvents>("Initializing PayPal provider registration binding events");


			GatewayProviderService.Saving += delegate(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> args)
			{
				var key = new Guid(Constants.PayPal.GatewayProviderKey);
				var provider = args.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);
				if (provider == null) return;

				provider.ExtendedData.SaveProviderSettings(new PayPalProviderSettings());

			};
		}
	}
}
