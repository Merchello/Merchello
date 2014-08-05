namespace Merchello.Core.Gateways.Notification.Smtp
{
    using System;
    using System.Linq;
    using Models;
    using Services;
    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;


    public class SmtpProviderEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<SmtpProviderEvents>("Initializing SMTP notification gateway provider registration binding events");

            GatewayProviderService.Saving += GatewayProviderServiceOnSaved;
        }

        private void GatewayProviderServiceOnSaved(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> args)
        {
            var key = new Guid("5F2E88D1-6D07-4809-B9AB-D4D6036473E9");
            var provider = args.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);
            if (provider == null) return;

            provider.ExtendedData.SaveSmtpProviderSettings(new SmtpNotificationGatewayProviderSettings());
        }
    }
}