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
        #region Fields

        /// <summary>
        /// The label selector.
        /// </summary>
        private static readonly PropertyInfo LabelSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Label);

        /// <summary>
        /// The full name selector.
        /// </summary>
        private static readonly PropertyInfo FullNameSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.FullName);

        /// <summary>
        /// The company selector.
        /// </summary>
        private static readonly PropertyInfo CompanySelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Company);

        /// <summary>
        /// The address type field selector.
        /// </summary>
        private static readonly PropertyInfo AddressTypeFieldSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, Guid>(x => x.AddressTypeFieldKey);

        /// <summary>
        /// The address 1 selector.
        /// </summary>
        private static readonly PropertyInfo Address1Selector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Address1);

        /// <summary>
        /// The address 2 selector.
        /// </summary>
        private static readonly PropertyInfo Address2Selector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Address2);

        /// <summary>
        /// The locality selector.
        /// </summary>
        private static readonly PropertyInfo LocalitySelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Locality);

        /// <summary>
        /// The region selector.
        /// </summary>
        private static readonly PropertyInfo RegionSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Region);

        /// <summary>
        /// The postal code selector.
        /// </summary>
        private static readonly PropertyInfo PostalCodeSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.PostalCode);

        /// <summary>
        /// The country code selector.
        /// </summary>
        private static readonly PropertyInfo CountryCodeSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.CountryCode);

        /// <summary>
        /// The phone selector.
        /// </summary>
        private static readonly PropertyInfo PhoneSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, string>(x => x.Phone);

        /// <summary>
        /// The customer key selector.
        /// </summary>
        private static readonly PropertyInfo CustomerKeySelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, Guid>(x => x.CustomerKey);

        /// <summary>
        /// The is default selector.
        /// </summary>
        private static readonly PropertyInfo IsDefaultSelector = ExpressionHelper.GetPropertyInfo<CustomerAddress, bool>(x => x.IsDefault);

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

        /// <summary>
        /// Gets customer id associated with the address
        /// </summary>
        [DataMember]
        public Guid CustomerKey
        {
            get
            {
                return _customerKey;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                        {
                            _customerKey = value;
                            return _customerKey;
                        },
                    _customerKey,
                    CustomerKeySelector);
            }
        }

        /// <summary>
        /// Gets or sets the descriptive label for the address
        /// </summary>
        [DataMember]
        public string Label
        {
            get
            {
                return _label;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _label = value;
                    return _label;
                }, 
                _label, 
                LabelSelector); 
            }
        }

        /// <summary>
        /// Gets or sets the full name for the address
        /// </summary>
        [DataMember]
        public string FullName
        {
            get
            {
                return _fullName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                { 
                    _fullName = value ;
                    return _label;
                }, 
                _fullName, 
                FullNameSelector);
            }
        }

        /// <summary>
        /// Gets or sets the company name associated with a company
        /// </summary>
        [DataMember]
        public string Company
        {
            get
            {
                return _company;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _company = value;
                    return _company;
                }, 
                _company, 
                CompanySelector);
            }
        }

        /// <summary>
        /// Gets or sets the address type indicator
        /// </summary>
        [DataMember]
        public Guid AddressTypeFieldKey
        {
            get
            {
                return _addressTypeFieldKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _addressTypeFieldKey = value;
                    return _addressTypeFieldKey;
                }, 
                _addressTypeFieldKey, 
                AddressTypeFieldSelector);
            }
        }

        /// <summary>
        /// Gets or sets the first address line
        /// </summary>
        [DataMember]
        public string Address1
        {
            get
            {
                return _address1;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _address1 = value;
                    return _address1;
                }, 
                _address1, 
                Address1Selector);
            }
        }

        /// <summary>
        /// Gets or sets the second address line 
        /// </summary>
        [DataMember]
        public string Address2
        {
            get
            {
                return _address2;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _address2 = value;
                    return _address2;
                }, 
                _address2, 
                Address2Selector);
            }
        }

        /// <summary>
        /// Gets or sets the locality or city of the address
        /// </summary>
        [DataMember]
        public string Locality
        {
            get
            {
                return _locality;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _locality = value;
                    return _locality;
                }, 
                _locality, 
                LocalitySelector);
            }
        }

        /// <summary>
        /// Gets or sets the region, state or province of the address
        /// </summary>
        [DataMember]
        public string Region
        {
            get
            {
                return _region;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                        {
                    _region = value;
                    return _region;
                }, 
                _region, 
                RegionSelector);
            }
        }

        /// <summary>
        /// Gets or sets the postal code of the address
        /// </summary>
        [DataMember]
        public string PostalCode
        {
            get
            {
                return _postalCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _postalCode = value;
                    return _postalCode;
                }, 
                _postalCode, 
                PostalCodeSelector);
            }
        }

        /// <summary>
        /// Gets or sets the country code of the address
        /// </summary>
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
                    SetPropertyValueAndDetectChanges(
                        o =>
                    {
                        _countryCode = value;
                        return _countryCode;
                    }, 
                    _countryCode, 
                    CountryCodeSelector);
                }
            }
        }

        /// <summary>
        /// Gets or sets the phone number associated with the address
        /// </summary>
        /// <remarks>
        /// This is sometimes required by shipping providers
        /// </remarks>
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
                    SetPropertyValueAndDetectChanges(
                        o =>
                    {
                        _phone = value;
                        return _phone;
                    }, 
                    _phone, 
                    PhoneSelector);
                }
            }
        }

        /// <summary>
        /// Gets or sets the AddressType
        /// </summary>
        /// <remarks>
        /// This property only allows internally defined AddressTypes to be set.  No Custom types.  These will have
        /// to be set through the AddressTypeFieldKey property directly.
        /// </remarks>
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

        /// <summary>
        /// Gets or sets a value indicating whether is default.
        /// </summary>
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
                    SetPropertyValueAndDetectChanges(
                        o =>
                        {
                            _isDefault = value;
                            return _isDefault;
                        },
                    _isDefault,
                    IsDefaultSelector);
                }
            }
        }
    }
}