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
        private Guid _warehouseKey;

        public ShipCountry(Guid warehouseKey, string countryCode) 
            : this(warehouseKey, countryCode, new ShipMethodCollection())
        { }

        public ShipCountry(Guid warehouseKey, string countryCode, ShipMethodCollection shipMethods)
            : this(warehouseKey, countryCode, new List<IProvince>(), shipMethods)
        {}

        internal ShipCountry(Guid warehouseKey, string countryCode, IEnumerable<IProvince> provinces, ShipMethodCollection shipMethods)
            : base(countryCode, provinces)
        {
            Mandate.ParameterCondition(warehouseKey != Guid.Empty, "warehouseKey");
            Mandate.ParameterNotNull(shipMethods, "shipMethods");

            _warehouseKey = warehouseKey;
            _shipMethodCollection = shipMethods;
        }

        private static readonly PropertyInfo WarehouseKeySelector = ExpressionHelper.GetPropertyInfo<ShipCountry, Guid>(x => x.WarehouseKey);
        private static readonly PropertyInfo ShipMethodsChangedSelector = ExpressionHelper.GetPropertyInfo<ShipCountry, ShipMethodCollection>(x => x.ShipMethods);

        private void ShipMethodsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ShipMethodsChangedSelector);
        }

        /// <summary>
        /// The warehouse key
        /// </summary>
        [DataMember]
        public Guid WarehouseKey
        {
            get { return _warehouseKey; }
            internal set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _warehouseKey = value;
                    return _warehouseKey;
                }, _warehouseKey, WarehouseKeySelector); 
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