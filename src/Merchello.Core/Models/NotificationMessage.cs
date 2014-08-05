using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a notification message
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class NotificationMessage : Entity, INotificationMessage
    {
        private string _name;
        private string _description;
        private string _bodyText;
        private string _fromAddress;
        private string _replyTo;
        private int _maxLength;
        private bool _bodyTextIsFilePath;
        private Guid? _monitorKey;
        private readonly Guid _methodKey;
        private string _recipients;
        private bool _sendToCustomer;
        private bool _disabled;

        public NotificationMessage(Guid notificationMethodKey, string name, string fromAddress)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(notificationMethodKey), "notificationMethodKey");
            Mandate.ParameterNotNullOrEmpty(fromAddress, "from");
            Mandate.ParameterNotNullOrEmpty(name, "name");

            _methodKey = notificationMethodKey;
            _fromAddress = fromAddress;
            _name = name;
            _maxLength = int.MaxValue;
        }

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.Name);
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.Description);
        private static readonly PropertyInfo FromSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.FromAddress);
        private static readonly PropertyInfo ReplyToSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.ReplyTo);
        private static readonly PropertyInfo MaxLengthSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, int>(x => x.MaxLength);
        private static readonly PropertyInfo MessageSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.BodyText);
        private static readonly PropertyInfo MessageIsFilePathSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, bool>(x => x.BodyTextIsFilePath);
        private static readonly PropertyInfo MonitorKeySelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, Guid?>(x => x.MonitorKey);
        private static readonly PropertyInfo RecipientsSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.Recipients);
        private static readonly PropertyInfo SendToCustomerSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, bool>(x => x.SendToCustomer);
        private static readonly PropertyInfo DisabledSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, bool>(x => x.Disabled);

        /// <summary>
        /// The <see cref="INotificationMethod"/> key
        /// </summary>
        public Guid MethodKey
        {
            get { return _methodKey; }
        }

        /// <summary>
        /// Gets or sets the name of the notification.
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _name; }
            internal set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _name = value;
                    return _name;
                }, _name, NameSelector);
            }
        }

        /// <summary>
        /// A brief description of the notification
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return _description; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _description = value;
                    return _description;
                }, _description, DescriptionSelector);
            }
        }

        /// <summary>
        /// The sender's from address
        /// </summary>
        [DataMember]
        public string FromAddress
        {
            get { return _fromAddress; }
            internal set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _fromAddress = value;
                    return _fromAddress;
                }, _fromAddress, FromSelector);
            }
        }

        /// <summary>
        /// The reply to 
        /// </summary>
        [DataMember]
        public string ReplyTo
        {
            get { return _replyTo; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _replyTo = value;
                    return _replyTo;
                }, _replyTo, ReplyToSelector);
            }
        }

        /// <summary>
        /// The path or text src
        /// </summary>
        [DataMember]
        public string BodyText
        {
            get { return _bodyText; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _bodyText = value;
                    return _bodyText;
                }, _bodyText, MessageSelector);
            }
        }

        /// <summary>
        /// The maximum length of the message
        /// </summary>
        [DataMember]
        public int MaxLength
        {
            get { return _maxLength; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _maxLength = value;
                    return _maxLength;
                }, _maxLength, MaxLengthSelector);
            }
        }

        /// <summary>
        /// True/false indicating whether or not the string value of Message is actually a path to a file to read
        /// </summary>
        public bool BodyTextIsFilePath
        {
            get { return _bodyTextIsFilePath; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _bodyTextIsFilePath = value;
                    return _bodyTextIsFilePath;
                }, _bodyTextIsFilePath, MessageIsFilePathSelector);
            }
        }

        /// <summary>
        /// Optional key for Notification Monitor
        /// </summary>
        [DataMember]
        public Guid? MonitorKey
        {
            get { return _monitorKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _monitorKey = value;
                    return _monitorKey;
                }, _monitorKey, MonitorKeySelector);
            }
        }

        /// <summary>
        /// The recipients of the notification
        /// </summary>
        [DataMember]
        public string Recipients
        {
            get { return _recipients; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _recipients = value;
                    return _recipients;
                }, _recipients, RecipientsSelector);
            }
        }

        /// <summary>
        /// True/false indicating whether or not this notification should be sent to the customer
        /// </summary>
        [DataMember]
        public bool SendToCustomer
        {
            get { return _sendToCustomer; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _sendToCustomer = value;
                    return _sendToCustomer;
                }, _sendToCustomer, SendToCustomerSelector);
            }
        }

        /// <summary>
        /// True/false indicating whether or not this notification is disabled
        /// </summary>
        [DataMember]
        public bool Disabled
        {
            get { return _disabled; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _disabled = value;
                    return _disabled;
                }, _disabled, DisabledSelector);
            }
        }
    }
}