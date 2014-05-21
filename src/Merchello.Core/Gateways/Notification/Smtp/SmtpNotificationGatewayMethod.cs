using System;
using System.Net.Mail;
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

            var client = new SmtpClient(_settings.Host);
            if (_settings.HasCredentials) client.Credentials = _settings.Credentials;
            if (_settings.EnableSsl) client.EnableSsl = true;


            //var send = client.SendAsync(msg);
            //send.ContinueWith(task =>
            //{
            //    if (task.IsCanceled)
            //    {
            //        LogHelper.Warn<SmtpNotificationGatewayMethod>("Send canceled");
            //    }

            //    if (!task.IsFaulted && task.Exception != null)
            //    {
            //        var ex = task.Exception.InnerExceptions.First();
            //        LogHelper.Error<SmtpNotificationGatewayMethod>("Failed sending email", ex);
            //    }
                                
            //});

            try
            {
                client.Send(msg);
                msg.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error<SmtpNotificationGatewayMethod>("Failed sending email", ex);
                return false;
            }                        
        }


    }
}