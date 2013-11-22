using System.Linq;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Extension methods for <see cref="IWarehouse"/>
    /// </summary>
    internal static class WarehouseExtensions
    {
        /// <summary>
        /// Helper extension that returns the first <see cref="IWarehouseCatalog"/> for the warehouse
        /// </summary>
        internal static IWarehouseCatalog DefaultCatalog(this IWarehouse warehouse)
        {
            return ((Warehouse) warehouse).InventoryCatalogs.FirstOrDefault();
        }
         
    }
}