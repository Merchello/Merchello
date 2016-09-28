namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    using Umbraco.Core;

    /// <summary>
    /// Represents a notification method
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class NotificationMethod : Entity, INotificationMethod
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The provider key.
        /// </summary>
        private readonly Guid _providerKey;

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The description.
        /// </summary>
        private string _description;

        /// <summary>
        /// The service code.
        /// </summary>
        private string _serviceCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMethod"/> class.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        internal NotificationMethod(Guid providerKey)
        {
            Ensure.ParameterCondition(!Guid.Empty.Equals(providerKey), "providerKey");
            _providerKey = providerKey;
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid ProviderKey
        {
            get
            {
                return _providerKey;
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
        public string ServiceCode
        {
            get
            {
                return _serviceCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _serviceCode, _ps.Value.ServiceCodeSelector);
            }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<NotificationMethod, string>(x => x.Name);

            /// <summary>
            /// The description selector.
            /// </summary>
            public readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<NotificationMethod, string>(x => x.Description);

            /// <summary>
            /// The service code selector.
            /// </summary>
            public readonly PropertyInfo ServiceCodeSelector = ExpressionHelper.GetPropertyInfo<NotificationMethod, string>(x => x.ServiceCode);
        }
    }
}