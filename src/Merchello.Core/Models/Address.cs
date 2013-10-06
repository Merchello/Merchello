using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    public class Address : IdEntity, IAddress
    {
        private readonly Guid _customerKey;
        private string _label;
        private string _fullName;
        private string _company;
        private Guid _addressTypeFieldKey;
        private string _address1;
        private string _address2;
        private string _locality;
        private string _region;
        private string _postalCode;
        private string _countryCode;
        private string _phone;

        internal Address(Guid customerPk, string label)
        {
            _customerKey = customerPk;
            _label = label;
        }

        ///TODO: We need to talk about the contstructor.  An empty address does not make a lot of sense.
        public Address(ICustomer customer, string label)
        {            
            Mandate.ParameterNotNull(customer, "customer");
            Mandate.ParameterNotNull(label, "label");
            _customerKey = customer.Key;
            _label = label;

        }
        
        private static readonly PropertyInfo LabelSelector = ExpressionHelper.GetPropertyInfo<Address, string>(x => x.Label);
        private static readonly PropertyInfo FullNameSelector = ExpressionHelper.GetPropertyInfo<Address, string>(x => x.FullName);
        private static readonly PropertyInfo CompanySelector = ExpressionHelper.GetPropertyInfo<Address, string>(x => x.Company);
        private static readonly PropertyInfo AddressTypeFieldSelector = ExpressionHelper.GetPropertyInfo<Address, Guid>(x => x.AddressTypeFieldKey);
        private static readonly PropertyInfo Address1Selector = ExpressionHelper.GetPropertyInfo<Address, string>(x => x.Address1);
        private static readonly PropertyInfo Address2Selector = ExpressionHelper.GetPropertyInfo<Address, string>(x => x.Address2);
        private static readonly PropertyInfo LocalitySelector = ExpressionHelper.GetPropertyInfo<Address, string>(x => x.Locality);
        private static readonly PropertyInfo RegionSelector = ExpressionHelper.GetPropertyInfo<Address, string>(x => x.Region);
        private static readonly PropertyInfo PostalCodeSelector = ExpressionHelper.GetPropertyInfo<Address, string>(x => x.PostalCode);
        private static readonly PropertyInfo CountryCodeSelector = ExpressionHelper.GetPropertyInfo<Address, string>(x => x.CountryCode);
        private static readonly PropertyInfo PhoneSelector = ExpressionHelper.GetPropertyInfo<Address, string>(x => x.Phone);

        /// <summary>
        /// The customer key (key) associated with the address
        /// </summary>
        [DataMember]
        public Guid CustomerKey
        {
            get { return _customerKey; }
        }

        /// <summary>
        /// The descriptive label for the address
        /// </summary>
        [DataMember]
        public string Label
        {
            get { return _label; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _label = value;
                    return _label;
                }, _label, LabelSelector); 
            }
        }

        /// <summary>
        /// The full name for the address
        /// </summary>
        [DataMember]
        public string FullName
        {
            get { return _fullName; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                { 
                    _fullName = value ;
                    return _label;
                }, _fullName, FullNameSelector);
            }
        }

        /// <summary>
        /// The company name associated with a company
        /// </summary>
        [DataMember]
        public string Company
        {
            get { return _company; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _company = value;
                    return _company;
                }, _company, CompanySelector);
            }
        }

        /// <summary>
        /// The address type indicator
        /// </summary>
        [DataMember]
        public Guid AddressTypeFieldKey
        {
            get { return _addressTypeFieldKey; }
            set 
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _addressTypeFieldKey = value;
                    return _addressTypeFieldKey;
                }, _addressTypeFieldKey, AddressTypeFieldSelector);
            }
        }

        /// <summary>
        /// The first address line
        /// </summary>
        [DataMember]
        public string Address1
        {
            get { return _address1; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _address1 = value;
                    return _address1;
                }, _address1, Address1Selector);
            }
        }

        /// <summary>
        /// The second address line 
        /// </summary>
        [DataMember]
        public string Address2
        {
            get { return _address2; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _address2 = value;
                    return _address2;
                }, _address2, Address2Selector);
            }
        }

        /// <summary>
        /// The locality or city of the address
        /// </summary>
        [DataMember]
        public string Locality
        {
            get { return _locality; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _locality = value;
                    return _locality;
                }, _locality, LocalitySelector);
            }
        }

        /// <summary>
        /// The region, state or province of the address
        /// </summary>
        [DataMember]
        public string Region
        {
            get { return _region; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _region = value;
                    return _region;
                }, _region, RegionSelector);
            }
        }

        /// <summary>
        /// The postal code of the address
        /// </summary>
        [DataMember]
        public string PostalCode
        {
            get { return _postalCode; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _postalCode = value;
                    return _postalCode;
                }, _postalCode, PostalCodeSelector);
            }
        }

        /// <summary>
        /// The country code of the address
        /// </summary>
        [DataMember]
        public string CountryCode
        {
            get { return _countryCode; }
            set
            {
                {
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _countryCode = value;
                        return _countryCode;
                    }, _countryCode, CountryCodeSelector);
                }
            }
        }

        /// <summary>
        /// The phone number associated with the address
        /// </summary>
        /// <remarks>
        /// This is sometimes required by shipping providers
        /// </remarks>
        [DataMember]
        public string Phone
        {
            get { return _phone; }
            set
            {
                {
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _phone = value;
                        return _phone;
                    }, _phone, PhoneSelector);
                }
            }
        }

        /// <summary>
        /// Gets/sets the AddressType
        /// </summary>
        /// <remarks>
        /// This property only allows internally defined AddressTypes to be set.  eg. no Custom types.  These will have
        /// to be set through the AddressTypeFieldKey property directly.
        /// </remarks>
        [DataMember]
        public AddressType AddressType
        {
            get { return EnumTypeFieldConverter.Address.GetTypeField(_addressTypeFieldKey); }
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



    }

}