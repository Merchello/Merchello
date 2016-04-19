namespace Merchello.Core.Gateways.Notification
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Formatters;
    using Models;

    using Umbraco.Core;
    using Umbraco.Core.IO;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Defines the base notification
    /// </summary>
    internal class FormattedNotificationMessage : IFormattedNotificationMessage
    {
        /// <summary>
        /// The notification message.
        /// </summary>
        private readonly INotificationMessage _notificationMessage;

        /// <summary>
        /// The formatter.
        /// </summary>
        private readonly IFormatter _formatter;

        /// <summary>
        /// The recipients.
        /// </summary>
        private readonly List<string> _recipients = new List<string>();

        /// <summary>
        /// The formatted message.
        /// </summary>
        private Lazy<string> _formattedMessage;


        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedNotificationMessage"/> class.
        /// </summary>
        /// <param name="notificationMessage">
        /// The notification message.
        /// </param>
        /// <param name="formatter">
        /// The formatter.
        /// </param>
        public FormattedNotificationMessage(INotificationMessage notificationMessage, IFormatter formatter)
        {
            Mandate.ParameterNotNull(formatter, "formatter");
            Mandate.ParameterNotNull(notificationMessage, "message");

            _notificationMessage = notificationMessage;
            _formatter = formatter;

            Initialize();
        }

        /// <summary>
        /// Gets the sender's From address
        /// </summary>
        public string From 
        {
            get { return _notificationMessage.FromAddress; }
        }

        /// <summary>
        /// Gets the optional ReplyTo address
        /// </summary>
        public string ReplyTo 
        {
            get { return _notificationMessage.ReplyTo; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Gets a list of recipients for the notification.
        /// </summary>
        /// <remarks>
        /// This could be email addresses, mailing addresses, mobile numbers
        /// </remarks>
        public IEnumerable<string> Recipients 
        {
            get { return _recipients; }
        }

        /// <summary>
        /// Gets a value indicating whether the notification should also be sent to the customer
        /// </summary>
        public bool SendToCustomer 
        {
            get { return _notificationMessage.SendToCustomer; }
        }


        /// <summary>
        /// Gets notification message body text
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
        /// Gets status of the formatted message
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
        /// Gets the <see cref="INotificationMessage"/>
        /// </summary>
        internal INotificationMessage NotificationMessage
        {
            get { return _notificationMessage; }
        }

        /// <summary>
        /// Adds a recipient to the send to list
        /// </summary>
        /// <param name="value">The recipient</param>
        public void AddRecipient(string value)
        {
            if (!_recipients.Contains(value)) _recipients.Add(value);
        }

        /// <summary>
        /// Removes a recipient from the send to list
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void RemoveRecipient(string value)
        {
            if (!_recipients.Contains(value)) return;
            _recipients.Remove(value);
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
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

        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            _formattedMessage = new Lazy<string>(() => _formatter.Format(GetMessage()));

            Name = _notificationMessage.Name;

            if (!_notificationMessage.Recipients.Any()) return;

            var tos = _notificationMessage.Recipients.Replace(',', ';');
            _recipients.AddRange(tos.Split(';').Select(x => x.Trim()));
        }
    }
}