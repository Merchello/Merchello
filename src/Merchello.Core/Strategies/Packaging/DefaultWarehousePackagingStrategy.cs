using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Merchello.Core.Models;
using Umbraco.Core.Logging;

namespace Merchello.Core.Strategies.Packaging
{
    /// <summary>
    /// Represents the default warehouse packaging strategy.  
    /// 
    /// The class is responsible for breaking a collection of products into one or more shipments.
    /// </summary>
    /// <remarks>
    /// 
    /// for initial version we are only exposing a single warehouse and a single warehouse catalog 
    /// TODO : add business logic to test for for catalog and warehouse variations.  This will also need to account for 
    /// various inventory possibilities
    /// 
    /// </remarks>
    public class DefaultWarehousePackagingStrategy : PackagingStrategyBase
    {
        public DefaultWarehousePackagingStrategy(LineItemCollection lineItemCollection, IAddress destination, Guid versionKey) 
            : base(lineItemCollection, destination, versionKey)
        { }

        public DefaultWarehousePackagingStrategy(IMerchelloContext merchelloContext, LineItemCollection lineItemCollection, IAddress destination, Guid versionKey) 
            : base(merchelloContext, lineItemCollection, destination, versionKey)
        { }

        /// <summary>
        /// Creates a collection of shipments for the current basket
        /// </summary>   
        public override IEnumerable<IShipment> PackageShipments()
        {
            // filter basket items for shippable items
            var shippableVisitor = new ShippableProductVisitor();            
            LineItemCollection.Accept(shippableVisitor);            

            if(!shippableVisitor.ShippableItems.Any()) return new List<IShipment>();
   
            // the origin address will be the default warehouse
            // For the initial version we are only exposing a single warehouse
            var warehouse = MerchelloContext.Services.WarehouseService.GetDefaultWarehouse();
            var origin = warehouse.AsAddress();
            
            //For the initial version we are only exposing a single shipment
            var shipment = new Shipment(origin, Destination)
                {
                    VersionKey = VersionKey // this is used in cache keys
                };

            // get the variants for each of the shippable line items
            var variants =
                   MerchelloContext.Services.ProductVariantService.GetByKeys(
                       shippableVisitor.ShippableItems.Select(x => x.ExtendedData.GetProductVariantKey()).Where(x => !Guid.Empty.Equals(x))
                       ).ToArray();

            foreach (var lineItem in shippableVisitor.ShippableItems)
            {
                // We need to know what Warehouse Catalog this product is associated with for shipping and inventory
                var variant = variants.FirstOrDefault(x => x.Key.Equals(lineItem.ExtendedData.GetProductVariantKey()));      
                if(variant == null) throw new InvalidOperationException("This packaging strategy cannot handle null ProductVariants");

                if (variant.CatalogInventories.FirstOrDefault() == null)
                {
                    LogHelper.Error<ShippableProductVisitor>("ProductVariant marked as shippable was not assoicated with a WarehouseCatalog.  Product was: " + variant.Key.ToString() + " -  " + variant.Name, new InvalidDataException());
                }
                else
                {                    
                    lineItem.ExtendedData.SetValue("merchWarehouseCatalogKey", variant.CatalogInventories.First().CatalogKey.ToString());                    
                    shipment.Items.Add(lineItem);    
                }
                          
            }

            return new List<IShipment> { shipment };
        }
    }
}