namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The warehouse.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Warehouse : Entity, IWarehouse
    {
        #region Fields

        /// <summary>
        /// The name selector.
        /// </summary>
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Name);

        /// <summary>
        /// The address 1 selector.
        /// </summary>
        private static readonly PropertyInfo Address1Selector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Address1);

        /// <summary>
        /// The address 2 selector.
        /// </summary>
        private static readonly PropertyInfo Address2Selector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Address2);

        /// <summary>
        /// The locality selector.
        /// </summary>
        private static readonly PropertyInfo LocalitySelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Locality);

        /// <summary>
        /// The region selector.
        /// </summary>
        private static readonly PropertyInfo RegionSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Region);

        /// <summary>
        /// The postal code selector.
        /// </summary>
        private static readonly PropertyInfo PostalCodeSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.PostalCode);

        /// <summary>
        /// The country code selector.
        /// </summary>
        private static readonly PropertyInfo CountryCodeSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.CountryCode);

        /// <summary>
        /// The phone selector.
        /// </summary>
        private static readonly PropertyInfo PhoneSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Phone);

        /// <summary>
        /// The email selector.
        /// </summary>
        private static readonly PropertyInfo EmailSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Email);

        /// <summary>
        /// The primary selector.
        /// </summary>
        private static readonly PropertyInfo PrimarySelector = ExpressionHelper.GetPropertyInfo<Warehouse, bool>(x => x.IsDefault);

        /// <summary>
        /// The catalogs.
        /// </summary>
        private readonly IEnumerable<IWarehouseCatalog> _catalogs;

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

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
        /// The email.
        /// </summary>
        private string _email;

        /// <summary>
        /// The is default.
        /// </summary>
        private bool _isDefault;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Warehouse"/> class.
        /// </summary>
        public Warehouse()
            : this(new List<IWarehouseCatalog>())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Warehouse"/> class.
        /// </summary>
        /// <param name="catalogs">
        /// The catalogs.
        /// </param>
        internal Warehouse(IEnumerable<IWarehouseCatalog> catalogs)
        {
            var warehouseCatalogs = catalogs as IWarehouseCatalog[] ?? catalogs.ToArray();
            Mandate.ParameterNotNull(warehouseCatalogs, "catalogs");
            _catalogs = warehouseCatalogs;
        }
        
        /// <summary>
        /// Gets or sets the name associated with the Warehouse
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
        /// Gets or sets the address1 associated with the Warehouse
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
        /// Gets or sets the address2 associated with the Warehouse
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
        /// Gets or sets the locality associated with the Warehouse
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
        /// Gets or sets the region associated with the Warehouse
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
        /// Gets or sets the postalCode associated with the Warehouse
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
        /// Gets or sets the country code associated with the address of the warehouse
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
        
        /// <summary>
        /// Gets or sets the phone number of the warehouse
        /// </summary>
        [DataMember]
        public string Phone
        {
            get
            {
                return _phone;
            }

            set
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

        /// <summary>
        /// Gets or sets the contact email address of the email address
        /// </summary>
        [DataMember]
        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _email = value;
                    return _email;
                }, 
                _email, 
                EmailSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this warehouse is the primary (or default) warehouse
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
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _isDefault = value;
                    return _isDefault;
                }, 
                _isDefault, 
                PrimarySelector);
            }
        }

        /// <summary>
        /// Gets a list of catalogs (used for inventory)
        /// </summary>
        [IgnoreDataMember]
        public IEnumerable<IWarehouseCatalog> WarehouseCatalogs 
        {
            get { return _catalogs; }
        }
    }
}