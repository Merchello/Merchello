using System.Collections.Generic;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Notification.Smtp;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Notification.Smtp
{
     ///<summary>
     /// Represents the SMTP Notification Gateway Provider
     ///</summary>
    [GatewayProviderActivation("5F2E88D1-6D07-4809-B9AB-D4D6036473E9", "SMTP Notification Provider", "SMTP Notification Provider")]
    [GatewayProviderEditor("AuthorizeNet configuration", "~/App_Plugins/Merchello/Modules/Settings/Notifications/Dialog/smtp.notifications.providersettings.html")]
    public class SmtpNotificationGatewayProvider : NotificationGatewayProviderBase, ISmtpNotificationGatewayProvider
    {
        public SmtpNotificationGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        { }

         public override IEnumerable<IGatewayResource> ListResourcesOffered()
         {
             throw new System.NotImplementedException();
         }
    }
}