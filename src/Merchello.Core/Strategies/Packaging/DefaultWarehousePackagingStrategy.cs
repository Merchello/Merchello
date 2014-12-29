namespace Merchello.Core.Strategies.Packaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Merchello.Core.Models;

    using Umbraco.Core.Logging;

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
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWarehousePackagingStrategy"/> class.
        /// </summary>
        /// <param name="lineItemCollection">
        /// The line item collection.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="versionKey">
        /// The version key.
        /// </param>
        public DefaultWarehousePackagingStrategy(
            LineItemCollection lineItemCollection,
            IAddress destination,
            Guid versionKey)
            : base(lineItemCollection, destination, versionKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWarehousePackagingStrategy"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="lineItemCollection">
        /// The line item collection.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="versionKey">
        /// The version key.
        /// </param>
        public DefaultWarehousePackagingStrategy(
            IMerchelloContext merchelloContext,
            LineItemCollection lineItemCollection,
            IAddress destination,
            Guid versionKey)
            : base(merchelloContext, lineItemCollection, destination, versionKey)
        {
        }

        /// <summary>
        /// Creates a collection of shipments for the current basket
        /// </summary>
        /// <returns>
        /// A collection of <see cref="IShipment"/>.
        /// </returns>
        public override IEnumerable<IShipment> PackageShipments()
        {
            // All packaged shipments will start with a shipment status of "Quoted" as these are being used for the Shipment Rate Quote
            // NOTE:  the "Packaging" status to indicate the shipment is physically being packaged/boxed up.
            var quoted = MerchelloContext.Services.ShipmentService.GetShipmentStatusByKey(Constants.DefaultKeys.ShipmentStatus.Quoted);

            // filter basket items for shippable items
            var shippableVisitor = new ShippableProductVisitor();            
            LineItemCollection.Accept(shippableVisitor);            

            if (!shippableVisitor.ShippableItems.Any()) return new List<IShipment>();
   
            // the origin address will be the default warehouse
            // For the initial version we are only exposing a single warehouse
            var warehouse = MerchelloContext.Services.WarehouseService.GetDefaultWarehouse();
            var origin = warehouse.AsAddress();
            
            ////For the initial version we are only exposing a single shipment
            var shipment = new Shipment(quoted, origin, Destination)
                {
                    VersionKey = VersionKey // this is used in cache keys
                };

            // get the variants for each of the shippable line items
            var variants =
                   MerchelloContext.Services.ProductVariantService.GetByKeys(
                       shippableVisitor.ShippableItems
                       .Select(x => x.ExtendedData.GetProductVariantKey())
                       .Where(x => !Guid.Empty.Equals(x))).ToArray();

            foreach (var lineItem in shippableVisitor.ShippableItems)
            {
                // We need to know what Warehouse Catalog this product is associated with for shipping and inventory
                var variant = variants.FirstOrDefault(x => x.Key.Equals(lineItem.ExtendedData.GetProductVariantKey()));
                if (variant == null) throw new InvalidOperationException("This packaging strategy cannot handle null ProductVariants");

                if (variant.CatalogInventories.FirstOrDefault() == null)
                {
                    LogHelper.Error<ShippableProductVisitor>(
                        "ProductVariant marked as shippable was not assoicated with a WarehouseCatalog.  Product was: "
                        + variant.Key.ToString() + " -  " + variant.Name,
                        new InvalidDataException());
                }
                else
                {
                    // TODO this needs to be refactored to look at the entire shipment
                    // since products could be in multiple catalogs which could have
                    // opposing shippng rules and we have the destination address.
                    lineItem.ExtendedData.SetValue(
                        Constants.ExtendedDataKeys.WarehouseCatalogKey,
                        variant.CatalogInventories.First().CatalogKey.ToString());
                    shipment.Items.Add(lineItem);
                }
            }

            return new List<IShipment> { shipment };
        }
    }
}