using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    internal class Notification : Entity, INotification
    {
        private string _name;
        private string _description;
        private string _src;
        private Guid? _ruleKey;
        private string _recipients;
        private bool _sendToCustomer;
        private bool _disabled;

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<Notification, string>(x => x.Name);
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<Notification, string>(x => x.Description);
        private static readonly PropertyInfo SrcSelector = ExpressionHelper.GetPropertyInfo<Notification, string>(x => x.Src);
        private static readonly PropertyInfo RuleKeySelector = ExpressionHelper.GetPropertyInfo<Notification, Guid?>(x => x.RuleKey);
        private static readonly PropertyInfo RecipientsSelector = ExpressionHelper.GetPropertyInfo<Notification, string>(x => x.Recipients);
        private static readonly PropertyInfo SendToCustomerSelector = ExpressionHelper.GetPropertyInfo<Notification, bool>(x => x.SendToCustomer);
        private static readonly PropertyInfo DisabledSelector = ExpressionHelper.GetPropertyInfo<Notification, bool>(x => x.Disabled);

        /// <summary>
        /// Gets or sets the name of the notification.
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
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
        /// The path or text src
        /// </summary>
        [DataMember]
        public string Src
        {
            get { return _src; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _src = value;
                    return _src;
                }, _src, SrcSelector);
            }
        }

        /// <summary>
        /// Optional key for Notification Trigger Rule
        /// </summary>
        [DataMember]
        public Guid? RuleKey
        {
            get { return _ruleKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _ruleKey = value;
                    return _ruleKey;
                }, _ruleKey, RuleKeySelector);
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