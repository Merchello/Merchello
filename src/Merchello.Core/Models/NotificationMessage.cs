namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    using Umbraco.Core;

    /// <summary>
    /// Defines a notification message
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class NotificationMessage : Entity, INotificationMessage
    {
        #region Fields

        /// <summary>
        /// The name selector.
        /// </summary>
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.Name);

        /// <summary>
        /// The description selector.
        /// </summary>
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.Description);

        /// <summary>
        /// The from selector.
        /// </summary>
        private static readonly PropertyInfo FromSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.FromAddress);

        /// <summary>
        /// The reply to selector.
        /// </summary>
        private static readonly PropertyInfo ReplyToSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.ReplyTo);

        /// <summary>
        /// The max length selector.
        /// </summary>
        private static readonly PropertyInfo MaxLengthSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, int>(x => x.MaxLength);

        /// <summary>
        /// The message selector.
        /// </summary>
        private static readonly PropertyInfo MessageSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.BodyText);

        /// <summary>
        /// The message is file path selector.
        /// </summary>
        private static readonly PropertyInfo MessageIsFilePathSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, bool>(x => x.BodyTextIsFilePath);

        /// <summary>
        /// The monitor key selector.
        /// </summary>
        private static readonly PropertyInfo MonitorKeySelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, Guid?>(x => x.MonitorKey);

        /// <summary>
        /// The recipients selector.
        /// </summary>
        private static readonly PropertyInfo RecipientsSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.Recipients);

        /// <summary>
        /// The send to customer selector.
        /// </summary>
        private static readonly PropertyInfo SendToCustomerSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, bool>(x => x.SendToCustomer);

        /// <summary>
        /// The disabled selector.
        /// </summary>
        private static readonly PropertyInfo DisabledSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, bool>(x => x.Disabled);

        /// <summary>
        /// The notification method key.
        /// </summary>
        private readonly Guid _methodKey;

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The description.
        /// </summary>
        private string _description;

        /// <summary>
        /// The body text.
        /// </summary>
        private string _bodyText;

        /// <summary>
        /// The from address.
        /// </summary>
        private string _fromAddress;

        /// <summary>
        /// The reply to.
        /// </summary>
        private string _replyTo;

        /// <summary>
        /// The max length.
        /// </summary>
        private int _maxLength;

        /// <summary>
        /// The body text is file path.
        /// </summary>
        private bool _bodyTextIsFilePath;

        /// <summary>
        /// The monitor key.
        /// </summary>
        private Guid? _monitorKey;

        /// <summary>
        /// The _recipients.
        /// </summary>
        private string _recipients;

        /// <summary>
        /// The send to customer.
        /// </summary>
        private bool _sendToCustomer;

        /// <summary>
        /// The disabled.
        /// </summary>
        private bool _disabled;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMessage"/> class.
        /// </summary>
        /// <param name="notificationMethodKey">
        /// The notification method key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="fromAddress">
        /// The from address.
        /// </param>
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


        /// <summary>
        /// Gets the <see cref="INotificationMethod"/> key
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
            get
            {
                return _name;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _name = value;
                    return _name;
                }, 
                _name, 
                NameSelector);
            }
        }

        /// <summary>
        /// Gets or sets the brief description of the notification
        /// </summary>
        [DataMember]
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _description = value;
                    return _description;
                }, 
                _description, 
                DescriptionSelector);
            }
        }

        /// <summary>
        /// Gets the sender's from address
        /// </summary>
        [DataMember]
        public string FromAddress
        {
            get
            {
                return _fromAddress;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _fromAddress = value;
                    return _fromAddress;
                }, 
                _fromAddress, 
                FromSelector);
            }
        }

        /// <summary>
        /// Gets or sets the reply to 
        /// </summary>
        [DataMember]
        public string ReplyTo
        {
            get
            {
                return _replyTo;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _replyTo = value;
                    return _replyTo;
                }, 
                _replyTo, 
                ReplyToSelector);
            }
        }

        /// <summary>
        /// Gets or sets the path or text source
        /// </summary>
        [DataMember]
        public string BodyText
        {
            get
            {
                return _bodyText;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _bodyText = value;
                    return _bodyText;
                }, 
                _bodyText, 
                MessageSelector);
            }
        }

        /// <summary>
        /// Gets or sets the maximum length of the message
        /// </summary>
        [DataMember]
        public int MaxLength
        {
            get
            {
                return _maxLength;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _maxLength = value;
                    return _maxLength;
                }, 
                _maxLength, 
                MaxLengthSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the string value of Message is actually a path to a file to read
        /// </summary>
        public bool BodyTextIsFilePath
        {
            get
            {
                return _bodyTextIsFilePath;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _bodyTextIsFilePath = value;
                    return _bodyTextIsFilePath;
                }, 
                _bodyTextIsFilePath, 
                MessageIsFilePathSelector);
            }
        }

        /// <summary>
        /// Gets or sets the optional key for Notification Monitor
        /// </summary>
        [DataMember]
        public Guid? MonitorKey
        {
            get
            {
                return _monitorKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _monitorKey = value;
                    return _monitorKey;
                }, 
                _monitorKey, 
                MonitorKeySelector);
            }
        }

        /// <summary>
        /// Gets or sets the recipients of the notification
        /// </summary>
        [DataMember]
        public string Recipients
        {
            get
            {
                return _recipients;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _recipients = value;
                    return _recipients;
                }, 
                _recipients, 
                RecipientsSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this notification should be sent to the customer
        /// </summary>
        [DataMember]
        public bool SendToCustomer
        {
            get
            {
                return _sendToCustomer;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _sendToCustomer = value;
                    return _sendToCustomer;
                }, 
                _sendToCustomer, 
                SendToCustomerSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this notification is disabled
        /// </summary>
        [DataMember]
        public bool Disabled
        {
            get
            {
                return _disabled;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                o =>
                {
                    _disabled = value;
                    return _disabled;
                }, 
                _disabled, 
                DisabledSelector);
            }
        }

        /// <summary>
        /// The shallow copy.
        /// </summary>
        /// <returns>
        /// The <see cref="INotificationMessage"/>.
        /// </returns>
        public INotificationMessage ShallowCopy()
        {
            return (INotificationMessage)this.MemberwiseClone();
        }
    }
}