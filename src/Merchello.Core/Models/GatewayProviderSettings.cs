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
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

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
        public Guid ProviderTfKey
        {
            get
            {
                return _providerTfKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _providerTfKey, _ps.Value.ProviderTfKeySelector);
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        [DataMember]
        public bool EncryptExtendedData
        {
            get
            {
                return _encryptExtendedData;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _encryptExtendedData, _ps.Value.EncryptExtendedDataSelector); 
            }
        }

        /// <inheritdoc/>
        public bool Activated 
        {
            get { return HasIdentity; }
        }

        /// <inheritdoc/>
        [DataMember]
        public GatewayProviderType GatewayProviderType 
        {
            get
            {
                return EnumTypeFieldConverter.GatewayProvider.GetTypeField(ProviderTfKey);
            }
        }

        /// <inheritdoc/>
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
        /// Handles the extended data collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(_ps.Value.ExtendedDataChangedSelector);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<GatewayProviderSettings, string>(x => x.Name);

            /// <summary>
            /// The description selector.
            /// </summary>
            public readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<GatewayProviderSettings, string>(x => x.Description);

            /// <summary>
            /// The provider type field key selector.
            /// </summary>
            public readonly PropertyInfo ProviderTfKeySelector = ExpressionHelper.GetPropertyInfo<GatewayProviderSettings, Guid>(x => x.ProviderTfKey);

            /// <summary>
            /// The extended data changed selector.
            /// </summary>
            public readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<GatewayProviderSettings, ExtendedDataCollection>(x => x.ExtendedData);

            /// <summary>
            /// The encrypt extended data selector.
            /// </summary>
            public readonly PropertyInfo EncryptExtendedDataSelector = ExpressionHelper.GetPropertyInfo<GatewayProviderSettings, bool>(x => x.EncryptExtendedData);

        }
    }
}