namespace Merchello.Core.Gateways.Notification.Smtp
{
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Services;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The smtp notification gateway provider.
    /// </summary>
    [GatewayProviderActivation("5F2E88D1-6D07-4809-B9AB-D4D6036473E9", "SMTP Notification Provider", "SMTP Notification Provider")]
    [GatewayProviderEditor("SMTP Notification Configuration", "~/App_Plugins/Merchello/Modules/Settings/GatewayProviders/Dialogs/smtp.notifications.providersettings.html")]
    public class SmtpNotificationGatewayProvider : NotificationGatewayProviderBase, ISmtpNotificationGatewayProvider
    {
        #region Resources

        /// <summary>
        /// The available resources.
        /// </summary>
        private static readonly IEnumerable<IGatewayResource> AvailableResources = new List<IGatewayResource>()
        {
            new GatewayResource("Email", "Email Notification")
        };

        #endregion

        public SmtpNotificationGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        { }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="IGatewayResource"/></returns>
        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            return AvailableResources.Where(x => NotificationMethods.All(y => y.ServiceCode != x.ServiceCode));
        }

         /// <summary>
         /// Creates a <see cref="INotificationGatewayMethod"/>
         /// </summary>
         /// <param name="gatewayResource">The <see cref="IGatewayResource"/> implemented by this method</param>
         /// <param name="name">The name of the notification method</param>
         /// <param name="serviceCode"></param>
         /// <returns></returns>
         public override INotificationGatewayMethod CreateNotificationMethod(IGatewayResource gatewayResource, string name, string serviceCode)
        {
            var attempt = GatewayProviderService.CreateNotificationMethodWithKey(GatewayProviderSettings.Key, name, serviceCode);

            if (attempt.Success) return new SmtpNotificationGatewayMethod(GatewayProviderService, attempt.Result, GatewayProviderSettings.ExtendedData);

            LogHelper.Error<NotificationGatewayProviderBase>(string.Format("Failed to create NotificationGatewayMethod GatewayResource: {0} , {1}", gatewayResource.Name, gatewayResource.ServiceCode), attempt.Exception);

            throw attempt.Exception;
        }

         /// <summary>
         /// Gets a collection of all <see cref="INotificationGatewayMethod"/>s for this provider
         /// </summary>
         /// <returns>A collection of <see cref="INotificationGatewayMethod"/></returns>
         public override IEnumerable<INotificationGatewayMethod> GetAllNotificationGatewayMethods()
         {
             return NotificationMethods.Select(method => new SmtpNotificationGatewayMethod(GatewayProviderService, method, ExtendedData));
         }
    }
}