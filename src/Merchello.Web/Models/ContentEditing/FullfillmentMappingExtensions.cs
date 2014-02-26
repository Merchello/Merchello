using System;
using System.Linq;
using Merchello.Core.Gateways.Shipping.FixedRate;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using System.Collections.Generic;
using Merchello.Core.Gateways.Shipping;

namespace Merchello.Web.Models.ContentEditing
{
    internal static class FullfillmentMappingExtensions
    {
        #region WarehouseDisplay

        internal static WarehouseDisplay ToWarehouseDisplay(this IWarehouse warehouse)
        {           
            return AutoMapper.Mapper.Map<WarehouseDisplay>(warehouse);
        }

        #endregion

        #region IWarehouse

        internal static IWarehouse ToWarehouse(this WarehouseDisplay warehouseDisplay, IWarehouse destination)
        {
            if (warehouseDisplay.Key != Guid.Empty)
            {
                destination.Key = warehouseDisplay.Key;
            }
            destination.Name = warehouseDisplay.Name;
            destination.Address1 = warehouseDisplay.Address1;
            destination.Address2 = warehouseDisplay.Address2;
            destination.Locality = warehouseDisplay.Locality;
            destination.Region = warehouseDisplay.Region;
            destination.PostalCode = warehouseDisplay.PostalCode;
            destination.CountryCode = warehouseDisplay.CountryCode;
            destination.Phone = warehouseDisplay.Phone;
            destination.Email = warehouseDisplay.Email;
            destination.IsDefault = warehouseDisplay.IsDefault;

            foreach (var warehouseCatalogDisplay in warehouseDisplay.WarehouseCatalogs)
            {
                IWarehouseCatalog destinationWarehouseCatalog;

                var matchingItems = destination.WarehouseCatalogs.Where(x => x.Key == warehouseCatalogDisplay.Key);
                if (matchingItems.Any())
                {
                    var existingWarehouseCatalog = matchingItems.First();
                    if (existingWarehouseCatalog != null)
                    {
                        destinationWarehouseCatalog = existingWarehouseCatalog;

                        destinationWarehouseCatalog = warehouseCatalogDisplay.ToWarehouseCatalog(destinationWarehouseCatalog);
                    }
                }
            }

            return destination;
        }

        #endregion

        #region WarehouseCatalogDisplay

        internal static WarehouseCatalogDisplay ToWarehouseCatalogDisplay(this IWarehouseCatalog warehouseCatalog)
        {            
            return AutoMapper.Mapper.Map<WarehouseCatalogDisplay>(warehouseCatalog);
        }

        #endregion

        #region IWarehouseCatalog

        internal static IWarehouseCatalog ToWarehouseCatalog(this WarehouseCatalogDisplay warehouseCatalogDisplay, IWarehouseCatalog destination)
        {
            if (warehouseCatalogDisplay.Key != Guid.Empty)
            {
                destination.Key = warehouseCatalogDisplay.Key;
            }
            destination.Name = warehouseCatalogDisplay.Name;
            destination.Description = warehouseCatalogDisplay.Description;

            return destination;
        }

        #endregion

        #region CatalogInventoryDisplay

        internal static CatalogInventoryDisplay ToCatalogInventoryDisplay(this ICatalogInventory catalogInventory)
        {            
            return AutoMapper.Mapper.Map<CatalogInventoryDisplay>(catalogInventory);
        }

        #endregion

        #region ICatalogInventory

        internal static ICatalogInventory ToCatalogInventory(this CatalogInventoryDisplay catalogInventoryDisplay, ICatalogInventory destination)
        {
            destination.Count = catalogInventoryDisplay.Count;
            destination.LowCount = catalogInventoryDisplay.LowCount;

            return destination;
        }

        #endregion

        #region ShipCountryDisplay

        internal static ShipCountryDisplay ToShipCountryDisplay(this IShipCountry shipCountry)
        {            
            return AutoMapper.Mapper.Map<ShipCountryDisplay>(shipCountry);
        }

        #endregion

        #region IShipCountry

        internal static IShipCountry ToShipCountry(this ShipCountryDisplay shipCountryDisplay, IShipCountry destination)
        {
            // May not be any mapping

            return destination;
        }

        #endregion

        #region GatewayProviderDisplay

        internal static GatewayProviderDisplay ToGatewayProviderDisplay(this IGatewayProvider gatewayProvider)
        {
            return AutoMapper.Mapper.Map<GatewayProviderDisplay>(gatewayProvider);
        }

        #endregion

        #region GatewayResourceDisplay

        internal static GatewayResourceDisplay ToGatewayProviderDisplay(this IGatewayResource gatewayResource)
        {
            return AutoMapper.Mapper.Map<GatewayResourceDisplay>(gatewayResource);
        }

        #endregion

        #region ShipGatewayProviderDisplay

        internal static ShippingGatewayProviderDisplay ToShipGatewayProviderDisplay(this IShippingGatewayProvider shipGatewayProvider)
        {
            return AutoMapper.Mapper.Map<ShippingGatewayProviderDisplay>(shipGatewayProvider);
        }

        #endregion

        #region ShipMethodDisplay

        internal static ShipMethodDisplay ToShipMethodDisplay(this IShipMethod shipMethod)
        {            
            return AutoMapper.Mapper.Map<ShipMethodDisplay>(shipMethod);
        }

        #endregion

        #region IShipMethod

        internal static IShipMethod ToShipMethod(this ShipMethodDisplay shipMethodDisplay, IShipMethod destination)
        {
            if (shipMethodDisplay.Key != Guid.Empty)
            {
                destination.Key = shipMethodDisplay.Key;
            }
            destination.Name = shipMethodDisplay.Name;
            destination.ServiceCode = shipMethodDisplay.ServiceCode;
            destination.Surcharge = shipMethodDisplay.Surcharge;
            destination.Taxable = shipMethodDisplay.Taxable;

            foreach (var shipProvinceDisplay in shipMethodDisplay.Provinces)
            {
                IShipProvince destinationShipProvince;

                var matchingItems = destination.Provinces.Where(x => x.Code == shipProvinceDisplay.Code);
                if (matchingItems.Count() > 0)
                {
                    var existingShipProvince = matchingItems.First();
                    if (existingShipProvince != null)
                    {
                        destinationShipProvince = existingShipProvince;

                        destinationShipProvince = shipProvinceDisplay.ToShipProvince(destinationShipProvince);
                    }
                }
            }

            return destination;
        }

        #endregion

        #region ShipProvinceDisplay

        internal static ShipProvinceDisplay ToShipProvinceDisplay(this IShipProvince shipProvince)
        {            
            return AutoMapper.Mapper.Map<ShipProvinceDisplay>(shipProvince);
        }

        #endregion

        #region IShipProvince

        internal static IShipProvince ToShipProvince(this ShipProvinceDisplay shipProvinceDisplay, IShipProvince destination)
        {
            destination.AllowShipping = shipProvinceDisplay.AllowShipping;
            destination.RateAdjustment = shipProvinceDisplay.RateAdjustment;
            destination.RateAdjustmentType = shipProvinceDisplay.RateAdjustmentType;

            return destination;
        }

        #endregion
        
        #region RateTableShipMethodDisplay

        internal static RateTableShipMethodDisplay ToRateTableShipMethodDisplay(this IFixedRateShipMethod shipFixedRateMethod)
        {
            return AutoMapper.Mapper.Map<RateTableShipMethodDisplay>(shipFixedRateMethod);
        }

        #endregion

        #region IRateTableShipMethod

        /// <summary>
        /// Maps changes made in the <see cref="RateTableShipMethodDisplay"/> to the <see cref="IFixedRateShipMethod"/>
        /// </summary>
        /// <param name="rateTableShipMethodDisplay">The <see cref="RateTableShipMethodDisplay"/> to map</param>
        /// <param name="destination">The <see cref="IFixedRateShipMethod"/> to have changes mapped to</param>
        /// <returns>The updated <see cref="IFixedRateShipMethod"/></returns>
        /// <remarks>
        /// 
        /// Note: after calling this mapping, the changes are still not persisted to the database as the .Save() method is not called.
        /// 
        /// * For testing you will have to use the static .Save(IGatewayProviderService ..., as MerchelloContext.Current will likely be null
        /// 
        /// </remarks>
        internal static IFixedRateShipMethod ToRateTableShipMethod(this RateTableShipMethodDisplay rateTableShipMethodDisplay, IFixedRateShipMethod destination)
        {


            return destination;
        }

        #endregion

        #region ShipRateTableDisplay

        internal static ShipRateTableDisplay ToShipRateTableDisplay(this IShippingFixedRateTable shippingFixedRateTable)
        {            
            return AutoMapper.Mapper.Map<ShipRateTableDisplay>(shippingFixedRateTable);
        }

        #endregion

        #region IShipRateTable

        /// <summary>
        /// Maps changes made in the <see cref="ShipRateTableDisplay"/> to the <see cref="IShippingFixedRateTable"/>
        /// </summary>
        /// <param name="shipRateTableDisplay">The <see cref="ShipRateTableDisplay"/> to map</param>
        /// <param name="destination">The <see cref="IShippingFixedRateTable"/> to have changes mapped to</param>
        /// <returns>The updated <see cref="IShippingFixedRateTable"/></returns>
        /// <remarks>
        /// 
        /// Note: after calling this mapping, the changes are still not persisted to the database as the .Save() method is not called.
        /// 
        /// * For testing you will have to use the static .Save(IGatewayProviderService ..., as MerchelloContext.Current will likely be null
        /// 
        /// </remarks>
        internal static IShippingFixedRateTable ToShipRateTable(this ShipRateTableDisplay shipRateTableDisplay, IShippingFixedRateTable destination)
        {

            // determine if any rows were deleted
            var missingRows =
                destination.Rows.Where(
                    persisted => !shipRateTableDisplay.Rows.Select(display => display.Key).Where(x => x != Guid.Empty).Contains(persisted.Key));

            foreach (var missing in missingRows)
            {
                destination.DeleteRow(missing);
            }

            foreach (var shipRateTierDisplay in shipRateTableDisplay.Rows)
            {

                // try to find the matching row
                var destinationTier = destination.Rows.FirstOrDefault(x => x.Key == shipRateTierDisplay.Key);
                                
                if (destinationTier != null)
                {
                    // update the tier information : note we can only update the Rate here!
                    // We need to remove the text boxes for the RangeLow and RangeHigh on any existing RateTable
                    destinationTier.Rate = shipRateTierDisplay.Rate;                    
                }
                else
                {
                    // this should be implemented in V1
                    destination.AddRow(shipRateTierDisplay.RangeLow, shipRateTierDisplay.RangeHigh, shipRateTierDisplay.Rate);
                }

                //IShipRateTier destinationShipRateTier;

                //var matchingItems = destination.Rows.Where(x => x.Key == shipRateTierDisplay.Key).ToArray();
                //if (matchingItems.Any())
                //{
                //    var existingshipRateTier = matchingItems.First();
                //    if (existingshipRateTier != null)
                //    {
                //        destinationShipRateTier = existingshipRateTier;

                //        destinationShipRateTier = shipRateTierDisplay.ToShipRateTier(destinationShipRateTier);
                //    }
                //}
                //else
                //{
                //    // Case if one was created in the back-office.  Not planned for v1
                //    destination.AddRow(shipRateTierDisplay.RangeLow, shipRateTierDisplay.RangeHigh, shipRateTierDisplay.Rate);
                //}
            }

            return destination;
        }

        #endregion

        #region ShipRateTierDisplay

        internal static ShipRateTierDisplay ToShipRateTierDisplay(this IShipRateTier shipRateTier)
        {            
            return AutoMapper.Mapper.Map<ShipRateTierDisplay>(shipRateTier);
        }

        #endregion

        #region IShipRateTier

        internal static IShipRateTier ToShipRateTier(this ShipRateTierDisplay shipRateTierDisplay, IShipRateTier destination)
        {
            if (shipRateTierDisplay.Key != Guid.Empty)
            {
                destination.Key = shipRateTierDisplay.Key;
            }
            destination.RangeHigh = shipRateTierDisplay.RangeHigh;
            destination.RangeLow = shipRateTierDisplay.RangeLow;
            destination.Rate = shipRateTierDisplay.Rate;

            return destination;
        }

        #endregion

        #region TaxProvinceDisplay

        internal static TaxProvinceDisplay ToTaxProvinceDisplay(this TaxMethod taxMethod)
        {            
            return AutoMapper.Mapper.Map<TaxProvinceDisplay>(taxMethod);
        }

        #endregion
    }
}
