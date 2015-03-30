using Merchello.Core.Formatters;

namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Models.MonitorModels;
    using Merchello.Core.Services;
    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Extension methods for <see cref="IShipment"/>
    /// </summary>
    public static class ShipmentExtensions
    {
        /// <summary>
        /// Utility extension to return a validated <see cref="IShipCountry"/> from a shipment.
        /// 
        /// For inventory and ship method selection purposes, <see cref="IShipment"/>s must be mapped to a single WarehouseCatalog (otherwise it should have been split into multiple shipments).
        /// 
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/></param>
        /// <param name="gatewayProviderService">The <see cref="IGatewayProviderService"/></param>
        /// <returns>An <see cref="Attempt{T}"/> where success result is the matching <see cref="IShipCountry"/></returns>
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
        /// <param name="shipment">The <see cref="IShipment"/></param>
        /// <param name="tryGetCached">
        /// If set true the strategy will try to get a quote from cache
        /// </param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public static IEnumerable<IShipmentRateQuote> ShipmentRateQuotes(this IShipment shipment, bool tryGetCached = true)
        {
            return shipment.ShipmentRateQuotes(MerchelloContext.Current, tryGetCached);
        }

        /// <summary>
        /// Returns a <see cref="IShipmentRateQuote"/> for a <see cref="IShipment"/> given the 'unique' key of the <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/></param>
        /// <param name="shipMethodKey">The GUID key as a string of the <see cref="IShipMethod"/></param>
        /// <returns>The <see cref="IShipmentRateQuote"/> for the shipment by the specific <see cref="IShipMethod"/> specified</returns>
        public static IShipmentRateQuote ShipmentRateQuoteByShipMethod(this IShipment shipment, string shipMethodKey)
        {
            return shipment.ShipmentRateQuoteByShipMethod(new Guid(shipMethodKey));
        }

        /// <summary>
        /// Returns a <see cref="IShipmentRateQuote"/> for a <see cref="IShipment"/> given the 'unique' key of the <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/></param>
        /// <param name="shipMethodKey">The GUID key of the <see cref="IShipMethod"/></param>
        /// <returns>The <see cref="IShipmentRateQuote"/> for the shipment by the specific <see cref="IShipMethod"/> specified</returns>
        public static IShipmentRateQuote ShipmentRateQuoteByShipMethod(this IShipment shipment, Guid shipMethodKey)
        {
            return shipment.ShipmentRateQuoteByShipMethod(MerchelloContext.Current, shipMethodKey);
        }

        /// <summary>
        /// Returns a string intended to be used as a 'Shipment Line Item' title or name
        /// </summary>
        /// <param name="shipmentRateQuote">
        /// The <see cref="IShipmentRateQuote"/> used to quote the line item
        /// </param>
        /// <returns>
        /// The shipment line item name
        /// </returns>
        public static string ShipmentLineItemName(this IShipmentRateQuote shipmentRateQuote)
        {
            return string.Format("Shipment - {0} - {1} items", shipmentRateQuote.ShipMethod.Name, shipmentRateQuote.Shipment.Items.Count);
        }

        /// <summary>
        /// The shipment rate quotes.
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="tryGetCached">
        /// If set true the strategy will try to get a quote from cache
        /// </param>
        /// <returns>
        /// The collection of <see cref="IShipmentRateQuote"/>
        /// </returns>
        internal static IEnumerable<IShipmentRateQuote> ShipmentRateQuotes(this IShipment shipment, IMerchelloContext merchelloContext, bool tryGetCached = true)
        {
            return merchelloContext.Gateways.Shipping.GetShipRateQuotesForShipment(shipment, tryGetCached);
        }

        /// <summary>
        /// Returns a <see cref="IShipmentRateQuote"/> for a <see cref="IShipment"/> given the 'unique' key of the <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="shipMethodKey">The GUID key of the <see cref="IShipMethod"/></param>
        /// <returns>The <see cref="IShipmentRateQuote"/> for the shipment by the specific <see cref="IShipMethod"/> specified</returns>
        internal static IShipmentRateQuote ShipmentRateQuoteByShipMethod(this IShipment shipment, IMerchelloContext merchelloContext, Guid shipMethodKey)
        {
            var shipMethod = ((ServiceContext)merchelloContext.Services).ShipMethodService.GetByKey(shipMethodKey);
            if (shipMethod == null) return null;

            // Get the gateway provider to generate the shipment rate quote
            var provider = merchelloContext.Gateways.Shipping.GetProviderByKey(shipMethod.ProviderKey);

            // get the GatewayShipMethod from the provider
            var gatewayShipMethod = provider.GetShippingGatewayMethodsForShipment(shipment).FirstOrDefault(x => x.ShipMethod.Key == shipMethodKey);

            return gatewayShipMethod == null ? null : provider.QuoteShipMethodForShipment(shipment, gatewayShipMethod);
        }

        /// <summary>
        /// Gets a collection of <see cref="IReplaceablePattern"/> for the shipment result notify model
        /// </summary>
        /// <param name="notifyModel">
        /// The <see cref="IShipmentResultNotifyModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ReplaceablePatterns}"/>.
        /// </returns>
        internal static IEnumerable<IReplaceablePattern> ReplaceablePatterns(this IShipmentResultNotifyModel notifyModel)
        {
            // TODO localization needed on pricing and datetime
            var shipment = notifyModel.Shipment;
            var patterns = new List<IReplaceablePattern>();
            patterns.AddRange(shipment.ReplaceablePatterns());
                                  
            patterns.AddRange(notifyModel.Invoice.ReplaceablePatterns());

            return patterns;
        }

        /// <summary>
        /// Adds shipment replaceable patters.
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IReplaceablePattern}"/>.
        /// </returns>
        internal static IEnumerable<IReplaceablePattern> ReplaceablePatterns(this IShipment shipment)
        {
            var patterns = new List<IReplaceablePattern>
            {
                ReplaceablePattern.GetConfigurationReplaceablePattern("ShippedDate", shipment.ShippedDate.ToShortDateString()),
                ReplaceablePattern.GetConfigurationReplaceablePattern("ShipToOrganization", shipment.ToOrganization),
                ReplaceablePattern.GetConfigurationReplaceablePattern("ShipToName", shipment.ToName),
                ReplaceablePattern.GetConfigurationReplaceablePattern("ShipToAddress1", shipment.ToAddress1),
                ReplaceablePattern.GetConfigurationReplaceablePattern("ShipToAddress2", shipment.ToAddress2),
                ReplaceablePattern.GetConfigurationReplaceablePattern("ShipToLocality", shipment.ToLocality),
                ReplaceablePattern.GetConfigurationReplaceablePattern("ShipToRegion", shipment.ToRegion),
                ReplaceablePattern.GetConfigurationReplaceablePattern("ShipToPostalCode", shipment.ToPostalCode),
                ReplaceablePattern.GetConfigurationReplaceablePattern("ShipToCountryCode", shipment.ToCountryCode),
                ReplaceablePattern.GetConfigurationReplaceablePattern("ShipToEmail", shipment.Email),
                ReplaceablePattern.GetConfigurationReplaceablePattern("ShipToPhone", shipment.Phone),
                ReplaceablePattern.GetConfigurationReplaceablePattern("ShipToOrganization", shipment.ToOrganization),
                ReplaceablePattern.GetConfigurationReplaceablePattern("TrackingCode", shipment.TrackingCode)
            };

            patterns.AddRange(shipment.LineItemReplaceablePatterns());

            return patterns;
        } 

        /// <summary>
        /// Clones a shipment
        /// </summary>
        /// <param name="org">
        /// The org.
        /// </param>
        /// <returns>
        /// The <see cref="IShipment"/>.
        /// </returns>
        /// <remarks>
        /// http://issues.merchello.com/youtrack/issue/M-458
        /// </remarks>
        internal static IShipment Clone(this IShipment org)
        {
            var lineItemCollection = new LineItemCollection();

            foreach (var li in org.Items)
            {
                lineItemCollection.Add(li.AsLineItemOf<OrderLineItem>());
            }

            return new Shipment(org.ShipmentStatus, org.GetOriginAddress(), org.GetDestinationAddress(), lineItemCollection)
                               {
                                   ShipmentNumberPrefix = org.ShipmentNumberPrefix,
                                   ShipmentNumber = org.ShipmentNumber,
                                   ShippedDate = org.ShippedDate,
                                   TrackingCode = org.TrackingCode,
                                   Carrier = org.Carrier,
                                   ShipMethodKey = org.ShipMethodKey,
                                   Phone = org.Phone,
                                   Email = org.Email
                               };
        }
    }
}