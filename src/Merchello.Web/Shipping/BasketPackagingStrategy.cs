using System.Collections.Generic;
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
    public class BasketPackagingStrategy : IBasketPackagingStrategy
    {
        private readonly IBasket _basket;
        private readonly IAddress _destination;

        public BasketPackagingStrategy(IBasket basket, IAddress destination)
        {
            Mandate.ParameterNotNull(basket, "basket");
            Mandate.ParameterNotNull(destination, "destination");

            _basket = basket;
            _destination = destination;
        }

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
        public IEnumerable<IShipment> PackageShipments()
        {
            // filter basket items for shippable items
            var shippableVisitor = new ShippableProductVisitor();            
            _basket.Accept(shippableVisitor);            

    

            // the origin address will be the default warehouse
            var origin = MerchelloContext.Current.Services.WarehouseService.GetDefaultWarehouse().AsAddress();

            //For the initial version we are only exposing a single shipment
            var shipment = new Shipment(origin, _destination);
            shippableVisitor.ShippableItems.ForEach(shipment.Items.Add);

            return new List<IShipment> { shipment};
        }
    }
}