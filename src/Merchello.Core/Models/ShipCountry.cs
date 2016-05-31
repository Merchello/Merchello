namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Umbraco.Core;

    /// <summary>
    /// Represents a country associated with a warehouse
    /// </summary>
    public class ShipCountry : CountryBase, IShipCountry
    {
        private Guid _catalogKey;


        public ShipCountry(Guid catalogKey, ICountry country)
            : this(catalogKey, country.CountryCode, country.Provinces)
        {}

        internal ShipCountry(Guid catalogKey, string countryCode, IEnumerable<IProvince> provinces)
            : base(countryCode, provinces)
        {
            Mandate.ParameterCondition(catalogKey != Guid.Empty, "catalogKey");

            _catalogKey = catalogKey;
        }

        private static readonly PropertyInfo CatalogKeySelector = ExpressionHelper.GetPropertyInfo<ShipCountry, Guid>(x => x.CatalogKey);


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
        /// True/false indicating whether or not this <see cref="IShipCountry"/> defines a province collection.
        /// </summary>
        public bool HasProvinces {
            get { return Provinces.Any(); }
        }
    }
}