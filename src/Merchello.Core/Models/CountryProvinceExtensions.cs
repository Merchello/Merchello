namespace Merchello.Core.Models
{
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Extension methods for the generic <see cref="ICountry"/> and <see cref="IProvince"/>
    /// </summary>
    public static class CountryProvinceExtensions
    {
        /// <summary>
        /// Converts a collection of <see cref="IProvince"/> to a new collection of <see cref="IShipProvince"/>
        /// </summary>
        /// <param name="provinces">A collection of <see cref="IProvince"/></param>
        /// <returns>A collection of <see cref="IShipProvince"/></returns>
        public static ProvinceCollection<IShipProvince> ToShipProvinceCollection(this IEnumerable<IProvince> provinces)
        {
            var provinceCollection = new ProvinceCollection<IShipProvince>();
            foreach (var p in provinces)
            {
                provinceCollection.Add(new ShipProvince(p.Code, p.Name));
            }

            return provinceCollection;
        }

        /// <summary>
        /// Converts a collection of <see cref="IProvince"/> to a new collection of <see cref="ITaxProvince"/>
        /// </summary>
        /// <param name="provinces">A collection of <see cref="IProvince"/></param>
        /// <returns>A collection of <see cref="ITaxProvince"/></returns>
        public static ProvinceCollection<ITaxProvince> ToTaxProvinceCollection(this IEnumerable<IProvince> provinces)
        {
            var provinceCollection = new ProvinceCollection<ITaxProvince>();
            foreach (var p in provinces)
            {
                provinceCollection.Add(new TaxProvince(p.Code, p.Name));
            }

            return provinceCollection;
        }
    }
}