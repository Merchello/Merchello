namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a notification message
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class NotificationMessage : Entity, INotificationMessage
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

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
            Ensure.ParameterCondition(!Guid.Empty.Equals(notificationMethodKey), "notificationMethodKey");
            Ensure.ParameterNotNullOrEmpty(fromAddress, "from");
            Ensure.ParameterNotNullOrEmpty(name, "name");

            _methodKey = notificationMethodKey;
            _fromAddress = fromAddress;
            _name = name;
            _maxLength = int.MaxValue;
        }


        /// <inheritdoc/>
        [DataMember]
        public Guid MethodKey
        {
            get
            {
                return _methodKey;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _name, _ps.Value.NameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _description, _ps.Value.DescriptionSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string FromAddress
        {
            get
            {
                return _fromAddress;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(value, ref _fromAddress, _ps.Value.FromSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string ReplyTo
        {
            get
            {
                return _replyTo;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _replyTo, _ps.Value.ReplyToSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string BodyText
        {
            get
            {
                return _bodyText;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _bodyText, _ps.Value.MessageSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public int MaxLength
        {
            get
            {
                return _maxLength;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _maxLength, _ps.Value.MaxLengthSelector);
            }
        }

        /// <inheritdoc/>
        public bool BodyTextIsFilePath
        {
            get
            {
                return _bodyTextIsFilePath;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _bodyTextIsFilePath, _ps.Value.MessageIsFilePathSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid? MonitorKey
        {
            get
            {
                return _monitorKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _monitorKey, _ps.Value.MonitorKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Recipients
        {
            get
            {
                return _recipients;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _recipients, _ps.Value.RecipientsSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool SendToCustomer
        {
            get
            {
                return _sendToCustomer;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _sendToCustomer, _ps.Value.SendToCustomerSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Disabled
        {
            get
            {
                return _disabled;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _disabled, _ps.Value.DisabledSelector);
            }
        }

        /// <inheritdoc/>
        public INotificationMessage ShallowCopy()
        {
            return (INotificationMessage)this.MemberwiseClone();
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.Name);

            /// <summary>
            /// The description selector.
            /// </summary>
            public readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.Description);

            /// <summary>
            /// The from selector.
            /// </summary>
            public readonly PropertyInfo FromSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.FromAddress);

            /// <summary>
            /// The reply to selector.
            /// </summary>
            public readonly PropertyInfo ReplyToSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.ReplyTo);

            /// <summary>
            /// The max length selector.
            /// </summary>
            public readonly PropertyInfo MaxLengthSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, int>(x => x.MaxLength);

            /// <summary>
            /// The message selector.
            /// </summary>
            public readonly PropertyInfo MessageSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.BodyText);

            /// <summary>
            /// The message is file path selector.
            /// </summary>
            public readonly PropertyInfo MessageIsFilePathSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, bool>(x => x.BodyTextIsFilePath);

            /// <summary>
            /// The monitor key selector.
            /// </summary>
            public readonly PropertyInfo MonitorKeySelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, Guid?>(x => x.MonitorKey);

            /// <summary>
            /// The recipients selector.
            /// </summary>
            public readonly PropertyInfo RecipientsSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, string>(x => x.Recipients);

            /// <summary>
            /// The send to customer selector.
            /// </summary>
            public readonly PropertyInfo SendToCustomerSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, bool>(x => x.SendToCustomer);

            /// <summary>
            /// The disabled selector.
            /// </summary>
            public readonly PropertyInfo DisabledSelector = ExpressionHelper.GetPropertyInfo<NotificationMessage, bool>(x => x.Disabled);
        }
    }
}