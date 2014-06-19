using System.Linq;

namespace Merchello.Core.Gateways.Notification.Smtp
{
    using System;
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
        private readonly SmtpNotificationGatewayProviderSettings _settings;

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
                msg.To.Add(new MailAddress(to));
            }
            
            //// We want to send the email async
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (msg)
                    {
                        using (var sender = new SmtpClient(_settings.Host))
                        {
                            if (_settings.HasCredentials) sender.Credentials = _settings.Credentials;
                            if (_settings.EnableSsl) sender.EnableSsl = true;
                            sender.Send(msg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error<SmtpNotificationGatewayMethod>("SMTP provider failed sending email", ex);
                }
            });
        }
    }
}