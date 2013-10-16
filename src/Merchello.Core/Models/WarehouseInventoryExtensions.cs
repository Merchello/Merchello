namespace Merchello.Core.Models
{
    /// <summary>
    /// Extension methods for <see cref="IWarehouseInventory"/> to simplify the <see cref="IProductVariant"/> to <see cref="IWarehouse"/> relationship
    /// and to provide more direct access to inventory functions.
    /// </summary>    
    /// <remarks>
    /// These require the MerchelloContext.Current singleton to be set
    /// </remarks>
    public static class WarehouseInventoryExtensions
    {
        /// <summary>
        /// Utility extension to retrieve the <see cref="IWarehouse"/>
        /// </summary>
        public static IWarehouse Warehouse(this IWarehouseInventory warehouseInventory)
        {
            return MerchelloContext.Current.Services.WarehouseService.GetById(warehouseInventory.WarehouseId);
        }


    }
}