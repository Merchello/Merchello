namespace Merchello.Core.Gateways.Notification
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Formatters;
    using Models;
    using Umbraco.Core.IO;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Defines the base notification
    /// </summary>
    internal class FormattedNotificationMessage : IFormattedNotificationMessage
    {
        private readonly INotificationMessage _notificationMessage;
        private readonly IFormatter _formatter;
        private Lazy<string> _formattedMessage;
        private readonly List<string> _recipients = new List<string>(); 

        public FormattedNotificationMessage(INotificationMessage notificationMessage, IFormatter formatter)
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

            Name = _notificationMessage.Name;

            if(!_notificationMessage.Recipients.Any()) return;
            
            var tos = _notificationMessage.Recipients.Replace(',', ';');
            _recipients.AddRange(tos.Split(';').Select(x => x.Trim()));
        }

        /// <summary>
        /// The <see cref="INotificationMessage"/>
        /// </summary>
        internal INotificationMessage NotificationMessage 
        {
            get { return _notificationMessage; }
        }

        /// <summary>
        /// The sender's From address
        /// </summary>
        public string From 
        {
            get { return _notificationMessage.FromAddress; }
        }

        /// <summary>
        /// The optional ReplyTo address
        /// </summary>
        public string ReplyTo 
        {
            get { return _notificationMessage.ReplyTo; }
        }

        public string Name { get; set; }


        /// <summary>
        /// A list of recipients for the notification.
        /// </summary>
        /// <remarks>
        /// This could be email addresses, mailing addresses, mobile numbers
        /// </remarks>
        public IEnumerable<string> Recipients 
        {
            get { return _recipients; }
        }

        /// <summary>
        /// True/false indicating if the notification should also be sent to the customer
        /// </summary>
        public bool SendToCustomer 
        {
            get { return _notificationMessage.SendToCustomer; }
        }


        /// <summary>
        /// The notification message body text
        /// </summary>
        public virtual string BodyText
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
        public virtual FormatStatus FormatStatus 
        {
            get
            {
                return _formattedMessage.Value.Length > _notificationMessage.MaxLength
                           ? FormatStatus.Truncated
                           : FormatStatus.Ok;
            }
        }

        /// <summary>
        /// Adds a recipient to the send to list
        /// </summary>
        /// <param name="value"></param>
        public void AddRecipient(string value)
        {
            if(!_recipients.Contains(value)) _recipients.Add(value);
        }

        /// <summary>
        /// Removes a recipient from the send to list
        /// </summary>
        public void RemoveRecipient(string value)
        {
            if (!_recipients.Contains(value)) return;
            _recipients.Remove(value);
        }


        private string GetMessage()
        {
            if (string.IsNullOrEmpty(_notificationMessage.BodyText)) return string.Empty;
            if (!_notificationMessage.BodyTextIsFilePath) return _notificationMessage.BodyText;

            try
            {
                return File.ReadAllText(IOHelper.FindFile(_notificationMessage.BodyText));
            }
            catch (Exception ex)
            {
                LogHelper.Error<FormattedNotificationMessage>("Failed to parse message from file", ex);
            }

            return string.Empty;
        }
    }
}