using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Extension methods for the generic <see cref="ICountry"/> and <see cref="IProvince"/>
    /// </summary>
    public static class CountryProvinceExtensions
    {
        /// <summary>
        /// Casts a collection of <see cref="IProvince"/> to a new collection of <see cref="IShipProvince"/>
        /// </summary>
        /// <param name="provinces"></param>
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
    }
}