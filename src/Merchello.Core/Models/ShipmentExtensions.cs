using System;
using System.IO;
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
                IsCommercial = shipment.ToIsCommercial
            };
        }

    }
}