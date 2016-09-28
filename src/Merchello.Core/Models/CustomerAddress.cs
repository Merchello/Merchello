namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// The customer address.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class CustomerAddress : Entity, ICustomerAddress
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

        /// <summary>
        /// The customer key.
        /// </summary>
        private Guid _customerKey;

        /// <summary>
        /// The label.
        /// </summary>
        private string _label;

        /// <summary>
        /// The full name.
        /// </summary>
        private string _fullName;

        /// <summary>
        /// The company.
        /// </summary>
        private string _company;

        /// <summary>
        /// The address type field key.
        /// </summary>
        private Guid _addressTypeFieldKey;

        /// <summary>
        /// The address 1.
        /// </summary>
        private string _address1;

        /// <summary>
        /// The address 2.
        /// </summary>
        private string _address2;

        /// <summary>
        /// The locality.
        /// </summary>
        private string _locality;

        /// <summary>
        /// The region.
        /// </summary>
        private string _region;

        /// <summary>
        /// The postal code.
        /// </summary>
        private string _postalCode;

        /// <summary>
        /// The country code.
        /// </summary>
        private string _countryCode;

        /// <summary>
        /// The phone.
        /// </summary>
        private string _phone;

        /// <summary>
        /// The is default.
        /// </summary>
        private bool _isDefault;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddress"/> class.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        public CustomerAddress(Guid customerKey)
        {            
            _customerKey = customerKey;

            // Default to a shipping address
            _addressTypeFieldKey = EnumTypeFieldConverter.Address.Shipping.TypeKey;
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid CustomerKey
        {
            get
            {
                return _customerKey;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(value, ref _customerKey, _ps.Value.CustomerKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Label
        {
            get
            {
                return _label;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _label, _ps.Value.LabelSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string FullName
        {
            get
            {
                return _fullName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _fullName, _ps.Value.FullNameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Company
        {
            get
            {
                return _company;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _company, _ps.Value.CompanySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid AddressTypeFieldKey
        {
            get
            {
                return _addressTypeFieldKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _addressTypeFieldKey, _ps.Value.AddressTypeFieldSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Address1
        {
            get
            {
                return _address1;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _address1, _ps.Value.Address1Selector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Address2
        {
            get
            {
                return _address2;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _address2, _ps.Value.Address2Selector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Locality
        {
            get
            {
                return _locality;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _locality, _ps.Value.LocalitySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Region
        {
            get
            {
                return _region;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _region, _ps.Value.RegionSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string PostalCode
        {
            get
            {
                return _postalCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _postalCode, _ps.Value.PostalCodeSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string CountryCode
        {
            get
            {
                return _countryCode;
            }

            set
            {
                {
                    SetPropertyValueAndDetectChanges(value, ref _countryCode, _ps.Value.CountryCodeSelector);
                }
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Phone
        {
            get
            {
                return _phone;
            }

            set
            {
                {
                    SetPropertyValueAndDetectChanges(value, ref _phone, _ps.Value.PhoneSelector);
                }
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public AddressType AddressType
        {
            get
            {
                return EnumTypeFieldConverter.Address.GetTypeField(_addressTypeFieldKey);
            }

            set
            {
                var reference = EnumTypeFieldConverter.Address.GetTypeField(value);
            
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                { 
                    // call through the property to flag the dirty property
                    AddressTypeFieldKey = reference.TypeKey;
                }
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool IsDefault
        {
            get
            {
                return _isDefault;
            }

            set
            {
                {
                    SetPropertyValueAndDetectChanges(value, ref _isDefault, _ps.Value.IsDefaultSelector);
                }
            }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The label selector.
            /// </summary>
            public readonly PropertyInfo LabelSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Label);

            /// <summary>
            /// The full name selector.
            /// </summary>
            public readonly PropertyInfo FullNameSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.FullName);

            /// <summary>
            /// The company selector.
            /// </summary>
            public readonly PropertyInfo CompanySelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Company);

            /// <summary>
            /// The address type field selector.
            /// </summary>
            public readonly PropertyInfo AddressTypeFieldSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, Guid>(x => x.AddressTypeFieldKey);

            /// <summary>
            /// The address 1 selector.
            /// </summary>
            public readonly PropertyInfo Address1Selector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Address1);

            /// <summary>
            /// The address 2 selector.
            /// </summary>
            public readonly PropertyInfo Address2Selector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Address2);

            /// <summary>
            /// The locality selector.
            /// </summary>
            public readonly PropertyInfo LocalitySelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Locality);

            /// <summary>
            /// The region selector.
            /// </summary>
            public readonly PropertyInfo RegionSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Region);

            /// <summary>
            /// The postal code selector.
            /// </summary>
            public readonly PropertyInfo PostalCodeSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.PostalCode);

            /// <summary>
            /// The country code selector.
            /// </summary>
            public readonly PropertyInfo CountryCodeSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.CountryCode);

            /// <summary>
            /// The phone selector.
            /// </summary>
            public readonly PropertyInfo PhoneSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Phone);

            /// <summary>
            /// The customer key selector.
            /// </summary>
            public readonly PropertyInfo CustomerKeySelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, Guid>(x => x.CustomerKey);

            /// <summary>
            /// The is default selector.
            /// </summary>
            public readonly PropertyInfo IsDefaultSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, bool>(x => x.IsDefault);

        }
    }
}