using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    internal class Warehouse : Entity, IWarehouse
    {
        private string _name;
        private string _address1;
        private string _address2;
        private string _locality;
        private string _region;
        private string _postalCode;
        private string _countryCode;
        private string _phone;
        private string _email;

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Name);  
        private static readonly PropertyInfo Address1Selector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Address1);  
        private static readonly PropertyInfo Address2Selector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Address2);  
        private static readonly PropertyInfo LocalitySelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Locality);  
        private static readonly PropertyInfo RegionSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Region);  
        private static readonly PropertyInfo PostalCodeSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.PostalCode);
        private static readonly PropertyInfo CountryCodeSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.CountryCode);
        private static readonly PropertyInfo PhoneSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Phone);
        private static readonly PropertyInfo EmailSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Email);  
        
        /// <summary>
        /// The name associated with the Warehouse
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
        /// The address1 associated with the Warehouse
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
        /// The address2 associated with the Warehouse
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
        /// The locality associated with the Warehouse
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
        /// The region associated with the Warehouse
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
        /// The postalCode associated with the Warehouse
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
        /// The country code associated with the address of the warehouse
        /// </summary>
        [DataMember]
        public string CountryCode
        {
            get { return _countryCode; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _countryCode = value;
                    return _countryCode;
                }, _countryCode, CountryCodeSelector); 
            }
        }    
        
        /// <summary>
        /// The phone number of the warehouse
        /// </summary>
        [DataMember]
        public string Phone
        {
            get { return _phone; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _phone = value;
                    return _phone;
                }, _phone, PhoneSelector); 
            }
        }

        /// <summary>
        /// The contact email address of the email address
        /// </summary>
        [DataMember]
        public string Email
        {
            get { return _email; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _email = value;
                    return _email;
                }, _email, EmailSelector);
            }
        }
    }

}