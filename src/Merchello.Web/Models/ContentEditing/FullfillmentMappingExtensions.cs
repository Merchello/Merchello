using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using System.Collections.Generic;
using Merchello.Core.Gateways.Shipping.RateTable;

namespace Merchello.Web.Models.ContentEditing
{
    internal static class FullfillmentMappingExtensions
    {
        #region WarehouseDisplay

        internal static WarehouseDisplay ToWarehouseDisplay(this IWarehouse warehouse)
        {
            AutoMapper.Mapper.CreateMap<IWarehouse, WarehouseDisplay>();
            AutoMapper.Mapper.CreateMap<IWarehouseCatalog, WarehouseCatalogDisplay>();

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
                if (matchingItems.Count() > 0)
                {
                    var existingWarehouseCatalog = matchingItems.First();
                    if (existingWarehouseCatalog != null)
                    {
                        destinationWarehouseCatalog = existingWarehouseCatalog;

                        destinationWarehouseCatalog = warehouseCatalogDisplay.ToWarehouseCatalog(destinationWarehouseCatalog);
                    }
                }
                else
                {
                    // Case if one was created in the back-office.  Not planned for v1

                    //destinationWarehouseCatalog = new WarehouseCatalog(warehouseDisplay.Key);

                    //destinationWarehouseCatalog = warehouseCatalogDisplay.ToWarehouseCatalog(destinationWarehouseCatalog);

                    //destination.WarehouseCatalogs.Add(destinationWarehouseCatalog);
                }
            }

            return destination;
        }

        #endregion

        #region WarehouseCatalogDisplay

        internal static WarehouseCatalogDisplay ToWarehouseCatalogDisplay(this IWarehouseCatalog warehouseCatalog)
        {
            AutoMapper.Mapper.CreateMap<IWarehouseCatalog, WarehouseCatalogDisplay>();

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
            AutoMapper.Mapper.CreateMap<ICatalogInventory, CatalogInventoryDisplay>();

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
            AutoMapper.Mapper.CreateMap<IShipCountry, ShipCountryDisplay>();
            AutoMapper.Mapper.CreateMap<IProvince, ShipProvinceDisplay>();

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

        #region ShipGatewayProviderDisplay

        //internal static ShipGatewayProviderDisplay ToShipGatewayProviderDisplay(this IShipGatewayProvider shipGatewayProvider)
        //{
        //    AutoMapper.Mapper.CreateMap<IShipGatewayProvider, ShipGatewayProviderDisplay>();

        //    return AutoMapper.Mapper.Map<ShipGatewayProviderDisplay>(shipGatewayProvider);
        //}

        #endregion

        #region ShipMethodDisplay

        internal static ShipMethodDisplay ToShipMethodDisplay(this IShipMethod shipMethod)
        {
            AutoMapper.Mapper.CreateMap<IShipMethod, ShipMethodDisplay>();
            AutoMapper.Mapper.CreateMap<IShipProvince, ShipProvinceDisplay>();

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
                else
                {
                    // Case if one was created in the back-office.
                }
            }

            return destination;
        }

        #endregion

        #region ShipProvinceDisplay

        internal static ShipProvinceDisplay ToShipProvinceDisplay(this IShipProvince shipProvince)
        {
            AutoMapper.Mapper.CreateMap<IShipProvince, ShipProvinceDisplay>();

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

        #region ShipRateTableDisplay

        internal static ShipRateTableDisplay ToShipRateTableDisplay(this IShipRateTable shipRateTable)
        {
            AutoMapper.Mapper.CreateMap<IShipRateTable, ShipRateTableDisplay>();
            AutoMapper.Mapper.CreateMap<IShipRateTier, ShipRateTierDisplay>();

            return AutoMapper.Mapper.Map<ShipRateTableDisplay>(shipRateTable);
        }

        #endregion

        #region IShipRateTable

        internal static IShipRateTable ToShipRateTable(this ShipRateTableDisplay shipRateTableDisplay, IShipRateTable destination)
        {
            foreach (var shipRateTierDisplay in shipRateTableDisplay.Rows)
            {
                IShipRateTier destinationShipRateTier;

                var matchingItems = destination.Rows.Where(x => x.Key == shipRateTierDisplay.Key);
                if (matchingItems.Count() > 0)
                {
                    var existingshipRateTier = matchingItems.First();
                    if (existingshipRateTier != null)
                    {
                        destinationShipRateTier = existingshipRateTier;

                        destinationShipRateTier = shipRateTierDisplay.ToShipRateTier(destinationShipRateTier);
                    }
                }
                else
                {
                    // Case if one was created in the back-office.  Not planned for v1
                    destination.AddRow(shipRateTierDisplay.RangeLow, shipRateTierDisplay.RangeHigh, shipRateTierDisplay.Rate);
                }
            }

            return destination;
        }

        #endregion

        #region ShipRateTierDisplay

        internal static ShipRateTierDisplay ToShipRateTierDisplay(this IShipRateTier shipRateTier)
        {
            AutoMapper.Mapper.CreateMap<IShipRateTier, ShipRateTierDisplay>();

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

        internal static TaxProvinceDisplay ToTaxProvinceDisplay(this CountryTaxRate countryTaxRate)
        {
            AutoMapper.Mapper.CreateMap<CountryTaxRate, TaxProvinceDisplay>();

            return AutoMapper.Mapper.Map<TaxProvinceDisplay>(countryTaxRate);
        }

        #endregion
    }
}
