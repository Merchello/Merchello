using System;
using System.Net.Mail;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Logging;

namespace Merchello.Core.Gateways.Notification.Smtp
{
    /// <summary>
    /// Representsa SmtpNotificationGatewayMethod
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
        public override bool PerformSend(IFormattedNotificationMessage message)
        {
            var msg = new MailMessage
            {
                From = new MailAddress(message.From),
                Subject = message.Name,
                Body =  message.BodyText,
                IsBodyHtml = true
            };

            foreach (var to in message.Recipients)
            {
                msg.To.Add(new MailAddress(to));
            }
            
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {

                    var client = new SmtpClient(_settings.Host);
                    if (_settings.HasCredentials) client.Credentials = _settings.Credentials;
                    if (_settings.EnableSsl) client.EnableSsl = true;

                    client.Send(msg);
                    msg.Dispose();
               
                }
                catch (Exception ex)
                {
                    LogHelper.Error<SmtpNotificationGatewayMethod>("Failed sending email", ex);                
                }
            });

            return true;
        }


    }
}