using System;
using System.Collections.Generic;
using System.IO;
using Merchello.Core.Models;
using System.Linq;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;

namespace Merchello.Core.Gateways.Notification
{
    /// <summary>
    /// Defines the base notification
    /// </summary>
    public abstract class NotificationGatewayMessageBase : INotificationGatewayMessage
    {
        private readonly INotificationMessage _notificationMessage;
        private readonly INotificationFormatter _formatter;
        private Lazy<string> _formattedMessage;

        protected NotificationGatewayMessageBase(INotificationMessage notificationMessage, INotificationFormatter formatter)
        {
            Mandate.ParameterNotNull(formatter, "formatter");
            Mandate.ParameterNotNull(notificationMessage, "message");

            _notificationMessage = notificationMessage;
            _formatter = formatter;

            Initialize();
        }

        private void Initialize()
        {
            _formattedMessage = new Lazy<string>(() => _formatter.Format(GetMessage()));
        }

        /// <summary>
        /// The <see cref="INotificationMessage"/>
        /// </summary>
        public INotificationMessage NotificationMessage {
            get { return _notificationMessage; }
        }

        /// <summary>
        /// A list of recipients for the notification.
        /// </summary>
        /// <remarks>
        /// This could be email addresses, mailing addresses, mobile numbers
        /// </remarks>
        public IEnumerable<string> Recipients {
            get
            {
                var recipients = _notificationMessage.Recipients.Replace(',', ';');
                return recipients.Split(';').Select(x => x.Trim());
            }
        }

        /// <summary>
        /// True/false indicating if the notification should also be sent to the customer
        /// </summary>
        public bool SendToCustomer {
            get { return _notificationMessage.SendToCustomer; }
        }


        /// <summary>
        /// The notification message
        /// </summary>
        public virtual string Message
        {
            get
            {
                return FormatStatus == FormatStatus.Ok
                           ? _formattedMessage.Value
                           : _formattedMessage.Value.Substring(0, _notificationMessage.MaxLength - 1);
            }
        }

        /// <summary>
        /// The status of the formatted message
        /// </summary>
        public virtual FormatStatus FormatStatus {
            get
            {
                return _formattedMessage.Value.Length > _notificationMessage.MaxLength
                           ? FormatStatus.Truncated
                           : FormatStatus.Ok;
            }
        }


        private string GetMessage()
        {
            if (!_notificationMessage.MessageIsFilePath) return _notificationMessage.Message;

            try
            {
                return File.ReadAllText(IOHelper.FindFile(_notificationMessage.Message));
            }
            catch (Exception ex)
            {
                LogHelper.Error<NotificationGatewayMessageBase>("Failed to parse message from file", ex);
            }
            return string.Empty;
        }

    }
}