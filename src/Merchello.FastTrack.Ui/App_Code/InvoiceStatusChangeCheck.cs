using Umbraco.Core;

namespace Merchello.FastTrack.Tests
{
    using Merchello.Core.Gateways.Notification.Smtp;
    using Merchello.Core.Services;

    using Umbraco.Core.Logging;

    public class InvoiceStatusChangeCheck : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            InvoiceService.StatusChanging += InvoiceService_StatusChanging;
            InvoiceService.StatusChanged += InvoiceService_StatusChanged;

            SmtpNotificationGatewayMethod.Sending += SmtpNotificatoinGatewayMethod_Sending;
        }

        private void SmtpNotificatoinGatewayMethod_Sending(SmtpNotificationGatewayMethod sender, Core.Events.ObjectEventArgs<System.Net.Mail.MailMessage> e)
        {
            var msg = e.EventObject;
            msg.BodyEncoding = System.Text.Encoding.UTF8;
        }

        private void InvoiceService_StatusChanging(IInvoiceService sender, Core.Events.StatusChangeEventArgs<Core.Models.IInvoice> e)
        {
            LogHelper.Info<InvoiceStatusChangeCheck>("Invoice status changing");
        }

        private void InvoiceService_StatusChanged(IInvoiceService sender, Core.Events.StatusChangeEventArgs<Core.Models.IInvoice> e)
        {
            LogHelper.Info<InvoiceStatusChangeCheck>("Invoice status changed");
        }
    }
}