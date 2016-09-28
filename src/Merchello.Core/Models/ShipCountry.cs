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
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The warehouse catalog key.
        /// </summary>
        private Guid _catalogKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipCountry"/> class.
        /// </summary>
        /// <param name="catalogKey">
        /// The catalog key.
        /// </param>
        /// <param name="country">
        /// The country.
        /// </param>
        public ShipCountry(Guid catalogKey, ICountry country)
            : this(catalogKey, country.CountryCode, country.Provinces)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipCountry"/> class.
        /// </summary>
        /// <param name="catalogKey">
        /// The catalog key.
        /// </param>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <param name="provinces">
        /// The provinces.
        /// </param>
        internal ShipCountry(Guid catalogKey, string countryCode, IEnumerable<IProvince> provinces)
            : base(countryCode, provinces)
        {
            Ensure.ParameterCondition(catalogKey != Guid.Empty, "catalogKey");

            _catalogKey = catalogKey;
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid CatalogKey
        {
            get
            {
                return _catalogKey;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(value, ref _catalogKey, _ps.Value.CatalogKeySelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool HasProvinces
        {
            get { return Provinces.Any(); }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The catalog key selector.
            /// </summary>
            public readonly PropertyInfo CatalogKeySelector = ExpressionHelper.GetPropertyInfo<ShipCountry, Guid>(x => x.CatalogKey);
        }
    }
}