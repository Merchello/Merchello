using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Notification.Smtp
{
     ///<summary>
     /// Represents the SMTP Notification Gateway Provider
     ///</summary>
    [GatewayProviderActivation("5F2E88D1-6D07-4809-B9AB-D4D6036473E9", "SMTP Notification Provider", "SMTP Notification Provider")]
    [GatewayProviderEditor("SMTP Notification Configuration", "~/App_Plugins/Merchello/Modules/Settings/Notifications/Dialog/smtp.notifications.providersettings.html")]
    public class SmtpNotificationGatewayProvider : NotificationGatewayProviderBase, ISmtpNotificationGatewayProvider
    {
        #region Resources
        
        private static readonly IEnumerable<IGatewayResource> AvailableResources = new List<IGatewayResource>()
        {
            new GatewayResource("Email", "Email Notification")
        };

        #endregion

        public SmtpNotificationGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        { }

         public override IEnumerable<IGatewayResource> ListResourcesOffered()
         {
             return AvailableResources.Where(x => NotificationMethods.Any(y => y.ServiceCode != x.ServiceCode));
         }

    }
}