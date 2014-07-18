namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// Defines a generic Gateway Provider
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class GatewayProviderSettings : Entity, IGatewayProviderSettings
    {
        #region Fields

        /// <summary>
        /// The name selector.
        /// </summary>
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<GatewayProviderSettings, string>(x => x.Name);

        /// <summary>
        /// The description selector.
        /// </summary>
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<GatewayProviderSettings, string>(x => x.Description);

        /// <summary>
        /// The provider type field key selector.
        /// </summary>
        private static readonly PropertyInfo ProviderTfKeySelector = ExpressionHelper.GetPropertyInfo<GatewayProviderSettings, Guid>(x => x.ProviderTfKey);

        /// <summary>
        /// The extended data changed selector.
        /// </summary>
        private static readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<GatewayProviderSettings, ExtendedDataCollection>(x => x.ExtendedData);

        /// <summary>
        /// The encrypt extended data selector.
        /// </summary>
        private static readonly PropertyInfo EncryptExtendedDataSelector = ExpressionHelper.GetPropertyInfo<GatewayProviderSettings, bool>(x => x.EncryptExtendedData);

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The description.
        /// </summary>
        private string _description;

        /// <summary>
        /// The provider type field key.
        /// </summary>
        private Guid _providerTfKey;

        /// <summary>
        /// The extended data.
        /// </summary>
        private ExtendedDataCollection _extendedData;

        /// <summary>
        /// The encrypt extended data.
        /// </summary>
        private bool _encryptExtendedData;

        #endregion


        /// <summary>
        /// Gets or sets the descriptive name or label for the provider
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
        /// Gets or sets the description of the provider
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
        /// Gets or sets the type field key for the provider
        /// </summary>
        [DataMember]
        public Guid ProviderTfKey
        {
            get
            {
                return _providerTfKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _providerTfKey = value;
                    return _providerTfKey;
                }, 
                _providerTfKey, 
                ProviderTfKeySelector);
            }
        }

        /// <summary>
        /// Gets the extended data for the provider
        /// </summary>
        [DataMember]
        public ExtendedDataCollection ExtendedData
        {
            get
            {
                return _extendedData;
            }

            internal set
            {
                _extendedData = value;
                _extendedData.CollectionChanged += ExtendedDataChanged;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or the ExtendedData collection should be encrypted before persisted.
        /// </summary>
        [DataMember]
        public bool EncryptExtendedData
        {
            get
            {
                return _encryptExtendedData;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _encryptExtendedData = value;
                    return _encryptExtendedData;
                }, 
                _encryptExtendedData, 
                EncryptExtendedDataSelector); 
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this provider is a "registered" and active provider.
        /// </summary>
        /// <remarks>
        /// Any persisted provider is an activated provider
        /// </remarks>
        public bool Activated 
        {
            get { return HasIdentity; }
        }

        /// <summary>
        /// Gets the type of the Gateway Provider
        /// </summary>
        [DataMember]
        public GatewayProviderType GatewayProviderType 
        {
            get
            {
                return EnumTypeFieldConverter.GatewayProvider.GetTypeField(ProviderTfKey);
            }
        }

        /// <summary>
        /// Method to call on entity saved when first added
        /// </summary>
        internal override void AddingEntity()
        {
            if (Key == Guid.Empty)
            {
                base.AddingEntity();
                return;
            }
            
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
        }

        /// <summary>
        /// The extended data changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ExtendedDataChangedSelector);
        }
    }
}