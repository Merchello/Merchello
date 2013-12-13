using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Web.Models;
using Umbraco.Core;

namespace Merchello.Web.Shipping
{
    /// <summary>
    /// Represents the default basket packaging strategy.  
    /// 
    /// The class is responsible for breaking the products in a basket into one or more shipments.
    /// </summary>
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
        /// <remarks>
        /// 
        /// for initial version we are only exposing a single warehouse and a single warehouse catalog 
        /// TODO : add business logic to test for for catalog and warehouse variations.  This will also need to account for 
        /// various inventory possibilities
        /// 
        /// </remarks>
        public override IEnumerable<IShipment> PackageShipments()
        {
            // filter basket items for shippable items
            var shippableVisitor = new ShippableProductVisitor();            
            Basket.Accept(shippableVisitor);            

            if(!shippableVisitor.ShippableItems.Any()) return new List<IShipment>();
   
            // the origin address will be the default warehouse
            var origin = MerchelloContext.Services.WarehouseService.GetDefaultWarehouse().AsAddress();

            //For the initial version we are only exposing a single shipment
            var shipment = new Shipment(origin, Destination);
            foreach (var lineItem in shippableVisitor.ShippableItems)
            {
                shipment.Items.Add(lineItem);
            }

            return new List<IShipment> { shipment };
        }
    }
}