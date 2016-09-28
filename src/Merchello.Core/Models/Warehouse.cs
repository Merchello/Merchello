namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <inheritdoc/>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Warehouse : Entity, IWarehouse
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

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
            Ensure.ParameterNotNull(warehouseCatalogs, "catalogs");
            _catalogs = warehouseCatalogs;
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
                SetPropertyValueAndDetectChanges(value, ref _countryCode, _ps.Value.CountryCodeSelector); 
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
                SetPropertyValueAndDetectChanges(value, ref _phone, _ps.Value.PhoneSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _email, _ps.Value.EmailSelector);
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
                SetPropertyValueAndDetectChanges(value, ref _isDefault, _ps.Value.IsDefaultSelector);
            }
        }

        /// <inheritdoc/>
        [IgnoreDataMember]
        public IEnumerable<IWarehouseCatalog> WarehouseCatalogs 
        {
            get
            {
                return _catalogs;
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
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Name);

            /// <summary>
            /// The address 1 selector.
            /// </summary>
            public readonly PropertyInfo Address1Selector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Address1);

            /// <summary>
            /// The address 2 selector.
            /// </summary>
            public readonly PropertyInfo Address2Selector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Address2);

            /// <summary>
            /// The locality selector.
            /// </summary>
            public readonly PropertyInfo LocalitySelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Locality);

            /// <summary>
            /// The region selector.
            /// </summary>
            public readonly PropertyInfo RegionSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Region);

            /// <summary>
            /// The postal code selector.
            /// </summary>
            public readonly PropertyInfo PostalCodeSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.PostalCode);

            /// <summary>
            /// The country code selector.
            /// </summary>
            public readonly PropertyInfo CountryCodeSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.CountryCode);

            /// <summary>
            /// The phone selector.
            /// </summary>
            public readonly PropertyInfo PhoneSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Phone);

            /// <summary>
            /// The email selector.
            /// </summary>
            public readonly PropertyInfo EmailSelector = ExpressionHelper.GetPropertyInfo<Warehouse, string>(x => x.Email);

            /// <summary>
            /// The primary selector.
            /// </summary>
            public readonly PropertyInfo IsDefaultSelector = ExpressionHelper.GetPropertyInfo<Warehouse, bool>(x => x.IsDefault);
        }
    }
}