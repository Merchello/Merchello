using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Core.Models
{
    public static class ShipmentExtensions
    {
        /// <summary>
        /// Utility extension to return a validated <see cref="IShipCountry"/> from a shipment.
        /// 
        /// For inventory and shipmethod selection purposes, <see cref="IShipment"/>s must be mapped to a single WarehouseCatalog (otherwise it should have been split into multiple shipments).
        /// 
        /// </summary>
        /// <param name="shipment"></param>
        /// <param name="gatewayProviderService"></param>
        /// <returns></returns>
        public static Attempt<IShipCountry> GetValidatedShipCountry(this IShipment shipment, IGatewayProviderService gatewayProviderService)
         {

             var visitor = new WarehouseCatalogValidationVisitor();
             shipment.Items.Accept(visitor);

             // quick validation of shipment
             if (visitor.CatalogCatalogValidationStatus != WarehouseCatalogValidationVisitor.CatalogValidationStatus.Ok)
             {
                 LogHelper.Error<ShippingGatewayProviderBase>("ShipMethods could not be determined for Shipment passed to GetAvailableShipMethodsForDestination method. Validator returned: " + visitor.CatalogCatalogValidationStatus, new ArgumentException("merchWarehouseCatalogKey"));
                 return visitor.CatalogCatalogValidationStatus ==
                        WarehouseCatalogValidationVisitor.CatalogValidationStatus.ErrorMultipleCatalogs
                            ? Attempt<IShipCountry>.Fail(
                                new InvalidDataException("Multiple CatalogKeys found in Shipment Items"))
                            : Attempt<IShipCountry>.Fail(new InvalidDataException("No CatalogKeys found in Shipment Items"));
             }

             return Attempt<IShipCountry>.Succeed(gatewayProviderService.GetShipCountry(visitor.WarehouseCatalogKey, shipment.ToCountryCode));
         }
    
        /// <summary>
        /// Gets an <see cref="IAddress"/> representing the origin address of the <see cref="IShipment"/>
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/></param>
        /// <returns>Returns a <see cref="IAddress"/></returns>
        public static IAddress GetOriginAddress(this IShipment shipment)
        {
            return new Address()
                {
                    Name = shipment.FromName,
                    Address1 = shipment.FromAddress1,
                    Address2 = shipment.FromAddress2,
                    Locality = shipment.FromLocality,
                    Region = shipment.FromRegion,
                    PostalCode = shipment.FromPostalCode,
                    CountryCode = shipment.FromCountryCode,
                    IsCommercial = shipment.FromIsCommercial
                };
        }

        /// <summary>
        /// Gets an <see cref="IAddress"/> representing the destination address of the <see cref="IShipment"/>
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/></param>
        /// <returns>Returns a <see cref="IAddress"/></returns>        
        public static IAddress GetDestinationAddress(this IShipment shipment)
        {
            return new Address()
            {
                Name = shipment.ToName,
                Address1 = shipment.ToAddress1,
                Address2 = shipment.ToAddress2,
                Locality = shipment.ToLocality,
                Region = shipment.ToRegion,
                PostalCode = shipment.ToPostalCode,
                CountryCode = shipment.ToCountryCode,
                IsCommercial = shipment.ToIsCommercial,
                Email = shipment.Email
            };
        }

        /// <summary>
        /// Returns a collection of <see cref="IShipmentRateQuote"/> from the various configured shipping providers
        /// </summary>
        /// <param name="shipment"><see cref="IShipment"/></param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public static IEnumerable<IShipmentRateQuote> ShipmentRateQuotes(this IShipment shipment)
        {
            return shipment.ShipmentRateQuotes(MerchelloContext.Current);
        }

        internal static IEnumerable<IShipmentRateQuote> ShipmentRateQuotes(this IShipment shipment, IMerchelloContext merchelloContext)
        {
            return merchelloContext.Gateways.Shipping.GetShipRateQuotesForShipment(shipment);
        }

        /// <summary>
        /// Returns a <see cref="IShipmentRateQuote"/> for a <see cref="IShipment"/> given the 'unique' key of the <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/></param>
        /// <param name="shipMethodKey">The Guid key as a string of the <see cref="IShipMethod"/></param>
        /// <returns>The <see cref="IShipmentRateQuote"/> for the shipment by the specific <see cref="IShipMethod"/> specified</returns>
        public static IShipmentRateQuote ShipmentRateQuoteByShipMethod(this IShipment shipment, string shipMethodKey)
        {
            return shipment.ShipmentRateQuoteByShipMethod(new Guid(shipMethodKey));
        }

        /// <summary>
        /// Returns a <see cref="IShipmentRateQuote"/> for a <see cref="IShipment"/> given the 'unique' key of the <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/></param>
        /// <param name="shipMethodKey">The Guid key of the <see cref="IShipMethod"/></param>
        /// <returns>The <see cref="IShipmentRateQuote"/> for the shipment by the specific <see cref="IShipMethod"/> specified</returns>
        public static IShipmentRateQuote ShipmentRateQuoteByShipMethod(this IShipment shipment, Guid shipMethodKey)
        {
            return shipment.ShipmentRateQuoteByShipMethod(MerchelloContext.Current, shipMethodKey);
        }

        /// <summary>
        /// Returns a <see cref="IShipmentRateQuote"/> for a <see cref="IShipment"/> given the 'unique' key of the <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="shipMethodKey">The Guid key of the <see cref="IShipMethod"/></param>
        /// <returns>The <see cref="IShipmentRateQuote"/> for the shipment by the specific <see cref="IShipMethod"/> specified</returns>
        internal static IShipmentRateQuote ShipmentRateQuoteByShipMethod(this IShipment shipment, IMerchelloContext merchelloContext, Guid shipMethodKey)
        {
            var shipMethod = ((ServiceContext) merchelloContext.Services).ShipMethodService.GetByKey(shipMethodKey);
            if (shipMethod == null) return null;

            // Get the gateway provider to generate the shipment rate quote
            var provider = merchelloContext.Gateways.Shipping.GetProviderByKey(shipMethod.ProviderKey);

            // get the GatewayShipMethod from the provider
            var gwShipMethod = provider.GetShippingGatewayMethodsForShipment(shipment).FirstOrDefault(x => x.ShipMethod.Key == shipMethodKey);

            if (gwShipMethod == null) return null;

            return provider.QuoteShipMethodForShipment(shipment, gwShipMethod);
        }

        /// <summary>
        /// Returns a string intended to be used as a 'Shipment Line Item' title or name
        /// </summary>
        /// <param name="shipmentRateQuote">The <see cref="IShipmentRateQuote"/> used to quote the line item</param>
        public static string ShimpentLineItemName(this IShipmentRateQuote shipmentRateQuote)
        {
            return string.Format("Shipment - {0} - {1} items", shipmentRateQuote.ShipMethod.Name, shipmentRateQuote.Shipment.Items.Count);
        }


        ///// <summary>
        ///// Returns the collection of <see cref="IOrderLineItem"/> associated with the <see cref="IShipment"/>
        ///// </summary>
        ///// <param name="shipment">The <see cref="IShipment"/></param>
        ///// <returns>The collection of <see cref="IOrderLineItem"/></returns>
        //public static IEnumerable<IOrderLineItem> OrderLineItems(this IShipment shipment)
        //{
        //    return shipment.OrderLineItems(MerchelloContext.Current);
        //}

        ///// <summary>
        ///// Returns the collection of <see cref="IOrderLineItem"/> associated with the <see cref="IShipment"/>
        ///// </summary>
        ///// <param name="shipment">The <see cref="IShipment"/></param>
        ///// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        ///// <returns>The collection of <see cref="IOrderLineItem"/></returns>
        //public static IEnumerable<IOrderLineItem> OrderLineItems(this IShipment shipment, IMerchelloContext merchelloContext)
        //{
        //    throw new NotImplementedException();
        //}
    }
}