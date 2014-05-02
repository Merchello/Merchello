using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a notification method
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class NotificationMethod : Entity, INotificationMethod
    {
        private string _name;
        private string _description;
        private readonly Guid _providerKey;
        private string _serviceCode;


        internal NotificationMethod(Guid providerKey)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(providerKey), "providerKey");
            _providerKey = providerKey;
        }

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<NotificationMethod, string>(x => x.Name);
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<NotificationMethod, string>(x => x.Description);
        private static readonly PropertyInfo ServiceCodeSelector = ExpressionHelper.GetPropertyInfo<NotificationMethod, string>(x => x.ServiceCode);

        /// <summary>
        /// The key associated with the gateway provider for the notification method
        /// </summary>
        [DataMember]
        public Guid ProviderKey
        {
            get { return _providerKey; }
        }

        /// <summary>
        /// The name assoicated with the notification method
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
        /// The description of the notification method
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
        /// The service code of the notification method
        /// </summary>
        [DataMember]
        public string ServiceCode
        {
            get { return _serviceCode; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _serviceCode = value;
                    return _serviceCode;
                }, _serviceCode, ServiceCodeSelector);
            }
        }
    }
}