using System;
using System.Collections.Generic;
using System.Data;
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
        /// <returns></returns>
         public static Attempt<IShipCountry> GetValidatedShipCountry(this IShipment shipment, IGatewayProviderService gatewayProviderService)
         {

             var visitor = new ShimpmentWarehouseCatalogValidationVisitor();
             shipment.Items.Accept(visitor);

             // quick validation of shipment
             if (visitor.CatalogValidationStatus != ShimpmentWarehouseCatalogValidationVisitor.ShipmentCatalogValidationStatus.Ok)
             {
                 LogHelper.Error<ShippingGatewayProviderBase>("ShipMethods could not be determined for Shipment passed to GetAvailableShipMethodsForDestination method. Validator returned: " + visitor.CatalogValidationStatus, new ArgumentException("merchWarehouseCatalogKey"));
                 return visitor.CatalogValidationStatus ==
                        ShimpmentWarehouseCatalogValidationVisitor.ShipmentCatalogValidationStatus.ErrorMultipleCatalogs
                            ? Attempt<IShipCountry>.Fail(
                                new InvalidDataException("Multiple CatalogKeys found in Shipment Items"))
                            : Attempt<IShipCountry>.Fail(new InvalidDataException("No CatalogKeys found in Shipment Items"));
             }

             return Attempt<IShipCountry>.Succeed(gatewayProviderService.GetShipCountry(visitor.WarehouseCatalogKey, shipment.ToCountryCode));
         }
    }
}