namespace Merchello.Core.Gateways.Notification.Smtp
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Models;
    using Services;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents a SmtpNotificationGatewayMethod
    /// </summary>
    public class SmtpNotificationGatewayMethod : NotificationGatewayMethodBase
    {
        /// <summary>
        /// The _settings.
        /// </summary>
        private readonly SmtpNotificationGatewayProviderSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpNotificationGatewayMethod"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="notificationMethod">
        /// The notification method.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        public SmtpNotificationGatewayMethod(IGatewayProviderService gatewayProviderService, INotificationMethod notificationMethod, ExtendedDataCollection extendedData) 
            : base(gatewayProviderService, notificationMethod)
        {
            Mandate.ParameterNotNull(extendedData, "extendedData");

            _settings = extendedData.GetSmtpProviderSettings();
        }

        /// <summary>
        /// Does the actual work of sending the <see cref="IFormattedNotificationMessage"/>
        /// </summary>
        /// <param name="message">The <see cref="IFormattedNotificationMessage"/> to be sent</param>
        public override void PerformSend(IFormattedNotificationMessage message)
        {
            if (!message.Recipients.Any()) return;

            var msg = new MailMessage
            {
                From = new MailAddress(message.From),
                Subject = message.Name,
                Body = message.BodyText,
                IsBodyHtml = true
            };

            foreach (var to in message.Recipients)
            {
                if (!string.IsNullOrEmpty(to))
                {
                    msg.To.Add(new MailAddress(to));
                }
            }
            
            //// We want to send the email async
            Task<bool> sendAsync = this.SendAsync(msg);
        }

        /// <summary>
        /// Sends an email asynchronously, logging any errors.
        /// </summary>
        /// <param name="msg">
        /// The <see cref="MailMessage"/> to send.
        /// </param>
        /// <param name="credentials">
        /// The <see cref="NetworkCredential"/>s containing identity credentials.
        /// </param>
        /// <returns>
        /// True if the email is sent successfully; otherwise, false.
        /// </returns>
        public async Task<bool> SendAsync(MailMessage msg, NetworkCredential credentials = null)
        {
            //// TODO ASP.NET 4.5
            //// HostingEnvironment.QueueBackgroundWorkItem(ct => SendMailAsync(msg));
            //// See http://blogs.msdn.com/b/webdev/archive/2014/06/04/queuebackgroundworkitem-to-reliably-schedule-and-run-long-background-process-in-asp-net.aspx

            try
            {
                // We want to send the email async
                using (var smtpClient = new SmtpClient(_settings.Host))
                {
                    if (_settings.HasCredentials) smtpClient.Credentials = _settings.Credentials;
                    if (_settings.EnableSsl) smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(msg);
                }

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error<SmtpNotificationGatewayMethod>("Merchello.Core.Gateways.Notification.Smtp.SmtpNotificationGatewayMethod  failed sending email", ex);
                return false;
            }
        }
    }
}