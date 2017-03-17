namespace Merchello.Core
{
    using System.Linq;

    using Merchello.Core.Models;

    /// <summary>
    /// Extension methods for <see cref="IWarehouse"/>.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Returns a Warehouse address as an <see cref="IAddress"/>
        /// </summary>
        /// <param name="warehouse">
        /// The warehouse.
        /// </param>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public static IAddress AsAddress(this IWarehouse warehouse)
        {
            return new Address()
            {
                Name = warehouse.Name,
                Address1 = warehouse.Address1,
                Address2 = warehouse.Address2,
                Locality = warehouse.Locality,
                Region = warehouse.Region,
                PostalCode = warehouse.PostalCode,
                CountryCode = warehouse.CountryCode,
                Phone = warehouse.Phone
            };
        }

        /// <summary>
        /// Helper extension that returns the first <see cref="IWarehouseCatalog"/> for the warehouse
        /// </summary>
        /// <param name="warehouse">
        /// The warehouse.
        /// </param>
        /// <returns>
        /// The <see cref="IWarehouseCatalog"/>.
        /// </returns>
        internal static IWarehouseCatalog DefaultCatalog(this IWarehouse warehouse)
        {
            return ((Warehouse)warehouse).WarehouseCatalogs.FirstOrDefault();
        }
    }
}
