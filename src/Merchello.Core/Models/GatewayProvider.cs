using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a generic Gateway Provider
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class GatewayProvider : Entity, IGatewayProvider
    {
        private string _name;
        private string _description;
        private Guid _providerTfKey;
        private string _typeFullName;
        private ExtendedDataCollection _extendedData;
        private bool _encryptExtendedData;       

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<GatewayProvider, string>(x => x.Name);
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<GatewayProvider, string>(x => x.Description);
        private static readonly PropertyInfo ProviderTfKeySelector = ExpressionHelper.GetPropertyInfo<GatewayProvider, Guid>(x => x.ProviderTfKey);
        private static readonly PropertyInfo TypeFullNameSelector = ExpressionHelper.GetPropertyInfo<GatewayProvider, string>(x => x.TypeFullName);
        private static readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<GatewayProvider, ExtendedDataCollection>(x => x.ExtendedData);
        private static readonly PropertyInfo EncryptExtendedDataSelector = ExpressionHelper.GetPropertyInfo<GatewayProvider, bool>(x => x.EncryptExtendedData);


        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ExtendedDataChangedSelector);
        }

        /// <summary>
        /// The descriptive name or label for the provider
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
        /// The description of the provider
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
        /// The type field key for the provider
        /// </summary>
        [DataMember]
        public Guid ProviderTfKey
        {
            get { return _providerTfKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _providerTfKey = value;
                    return _providerTfKey;
                }, _providerTfKey, ProviderTfKeySelector);
                
            }
        }

        /// <summary>
        /// The full Type name (used to instantiate the class)
        /// </summary>
        [DataMember]
        public string TypeFullName
        {
            get { return _typeFullName; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _typeFullName = value;
                    return _typeFullName;
                }, _typeFullName, TypeFullNameSelector); 
            }
        }

        /// <summary>
        /// Extended data for the provider
        /// </summary>
        [DataMember]
        public ExtendedDataCollection ExtendedData
        {
            get { return _extendedData; }
            internal set
            {
                _extendedData = value;
                _extendedData.CollectionChanged += ExtendedDataChanged;
            }
        }

        /// <summary>
        /// True/false indicating whether or the ExtendedData collection should be encrypted before persisted.
        /// </summary>
        [DataMember]
        public bool EncryptExtendedData
        {
            get { return _encryptExtendedData; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _encryptExtendedData = value;
                    return _encryptExtendedData;
                }, _encryptExtendedData, EncryptExtendedDataSelector); 
            }
        }

        /// <summary>
        /// True/false indicating whether or not this provider is a "registered" and active provider.
        /// </summary>
        /// <remarks>
        /// Any persisted provider is an activated provider
        /// </remarks>
        public bool Activated 
        {
            get { return HasIdentity; }
        }

        /// <summary>
        /// Enum type of the Gateway Provider
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
 
    }
}