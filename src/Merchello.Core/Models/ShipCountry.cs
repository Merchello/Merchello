namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a country associated with a warehouse
    /// </summary>
    public class ShipCountry : Entity, IShipCountry
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The <see cref="ICountry"/>.
        /// </summary>
        private readonly ICountry _country;

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
        {
            Ensure.ParameterCondition(catalogKey != Guid.Empty, "catalogKey");
            Ensure.ParameterNotNull(country, nameof(country));
            _country = country;
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

        /// <summary>
        /// Gets the country code.
        /// </summary>
        public string CountryCode
        {
            get
            {
                return _country.CountryCode;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return _country.Name;
            }
        }

        /// <summary>
        /// Gets the ISO.
        /// </summary>
        public int Iso
        {
            get
            {
                return _country.Iso;
            }
        }

        /// <summary>
        /// Gets the province label.
        /// </summary>
        public string ProvinceLabel
        {
            get
            {
                return _country.ProvinceLabel;
            }
        }

        /// <summary>
        /// Gets the provinces.
        /// </summary>
        public IEnumerable<IProvince> Provinces
        {
            get
            {
                return _country.Provinces;
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