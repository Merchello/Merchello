using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using System.Linq;

namespace Merchello.Core.Notifications
{
    /// <summary>
    /// Defines the base notification
    /// </summary>
    public abstract class NotificationBase : INotificationBase
    {
        private readonly INotificationMessage _notificationMessage;
        private readonly INotificationFormatter _formatter;
        private Lazy<string> _formattedMessage;

        protected NotificationBase(INotificationMessage notificationMessage, INotificationFormatter formatter)
        {
            Mandate.ParameterNotNull(formatter, "formatter");
            Mandate.ParameterNotNull(notificationMessage, "message");

            _notificationMessage = notificationMessage;
            _formatter = formatter;

            Initialize();
        }

        private void Initialize()
        {
            _formattedMessage = new Lazy<string>(() => _formatter.Format(_notificationMessage.Message));
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

    }
}