using System.Collections.Generic;
using System.IO;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Umbraco.Core.Logging;

namespace Merchello.Web.Workflow.Shipping
{
    /// <summary>
    /// Represents the default basket packaging strategy.  
    /// 
    /// The class is responsible for breaking the products in a basket into one or more shipments.
    /// </summary>
    /// <remarks>
    /// 
    /// for initial version we are only exposing a single warehouse and a single warehouse catalog 
    /// TODO : add business logic to test for for catalog and warehouse variations.  This will also need to account for 
    /// various inventory possibilities
    /// 
    /// </remarks>
    public class DefaultWarehousePackagingStrategy : BasketPackagingStrategyBase
    {
        public DefaultWarehousePackagingStrategy(IBasket basket, IAddress destination) 
            : base(basket, destination)
        { }

        public DefaultWarehousePackagingStrategy(IMerchelloContext merchelloContext, IBasket basket, IAddress destination) 
            : base(merchelloContext, basket, destination)
        { }

        /// <summary>
        /// Creates a collection of shipments for the current basket
        /// </summary>   
        public override IEnumerable<IShipment> PackageShipments()
        {
            // filter basket items for shippable items
            var shippableVisitor = new ShippableProductVisitor();            
            Basket.Accept(shippableVisitor);            

            if(!shippableVisitor.ShippableItems.Any()) return new List<IShipment>();
   
            // the origin address will be the default warehouse
            // For the initial version we are only exposing a single warehouse
            var warehouse = MerchelloContext.Services.WarehouseService.GetDefaultWarehouse();
            var origin = warehouse.AsAddress();
            
            //For the initial version we are only exposing a single shipment
            var shipment = new Shipment(origin, Destination);

            foreach (var lineItem in shippableVisitor.ShippableItems)
            {
                // We need to know what Warehouse Catalog this product is associated with for shipping and inventory
                var variant = ProductQuery.GetVariantDisplayByKey(lineItem.ExtendedData.GetProductVariantKey());
                if (variant.CatalogInventories.FirstOrDefault() == null)
                {
                    LogHelper.Error<ShippableProductVisitor>("ProductVariant marked as shippable was not assoicated with a WarehouseCatalog.  Product was: " + variant.Key.ToString() + " -  " + variant.Name, new InvalidDataException());
                }
                else
                {                    
                    lineItem.ExtendedData.SetValue("merchWarehouseCatalogKey", variant.CatalogInventories.First().CatalogKey.ToString());
                    // TODO note - this retains the it's ItemCacheLineItem.Key and the ContainerKey associated with the ItemCache
                    shipment.Items.Add(lineItem);    
                }
                          
            }

            return new List<IShipment> { shipment };
        }
    }
}