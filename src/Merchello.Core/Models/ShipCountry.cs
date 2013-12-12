using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a country associated with a warehouse
    /// </summary>
    public class ShipCountry : CountryBase, IShipCountry
    {
        private ShipMethodCollection _shipMethodCollection;
        private Guid _catalogKey;

        public ShipCountry(Guid catalogKey, ICountry country)
            : this(catalogKey, country, new ShipMethodCollection())
        { }

        public ShipCountry(Guid catalogKey, ICountry country, ShipMethodCollection shipMethods)
            : this(catalogKey, country.CountryCode, country.Provinces, shipMethods)
        {}

        internal ShipCountry(Guid catalogKey, string countryCode, IEnumerable<IProvince> provinces, ShipMethodCollection shipMethods)
            : base(countryCode, provinces)
        {
            Mandate.ParameterCondition(catalogKey != Guid.Empty, "catalogKey");
            Mandate.ParameterNotNull(shipMethods, "shipMethods");

            _catalogKey = catalogKey;
            _shipMethodCollection = shipMethods;
        }

        private static readonly PropertyInfo CatalogKeySelector = ExpressionHelper.GetPropertyInfo<ShipCountry, Guid>(x => x.CatalogKey);
        private static readonly PropertyInfo ShipMethodsChangedSelector = ExpressionHelper.GetPropertyInfo<ShipCountry, ShipMethodCollection>(x => x.ShipMethods);

        private void ShipMethodsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ShipMethodsChangedSelector);
        }

        /// <summary>
        /// The warehouse catalog key
        /// </summary>
        [DataMember]
        public Guid CatalogKey
        {
            get { return _catalogKey; }
            internal set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _catalogKey = value;
                    return _catalogKey;
                }, _catalogKey, CatalogKeySelector); 
            }
        }

        /// <summary>
        /// The ship methods associated with the country
        /// </summary>
        public ShipMethodCollection ShipMethods
        {
            get { return _shipMethodCollection; }
            internal set
            {
                _shipMethodCollection = value;
                _shipMethodCollection.CollectionChanged += ShipMethodsChanged;
            }
        }

    }
}