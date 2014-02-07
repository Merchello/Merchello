using Merchello.Core.Gateways.Shipping.RateTable;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Web.Models.ContentEditing;

namespace Merchello.Web
{
    internal static class AutoMapperMappings
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<ICatalogInventory, CatalogInventoryDisplay>();
            
            AutoMapper.Mapper.CreateMap<ICountry, CountryDisplay>();
            AutoMapper.Mapper.CreateMap<ICountryTaxRate, TaxProvinceDisplay>();
            
            // products
            AutoMapper.Mapper.CreateMap<IProduct, ProductDisplay>();
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            AutoMapper.Mapper.CreateMap<IProductOption, ProductOptionDisplay>();
            AutoMapper.Mapper.CreateMap<IProductVariant, ProductVariantDisplay>();

            AutoMapper.Mapper.CreateMap<IProvince, ProvinceDisplay>();

            // shipping            
            AutoMapper.Mapper.CreateMap<IShipCountry, ShipCountryDisplay>();
            AutoMapper.Mapper.CreateMap<IShipMethod, ShipMethodDisplay>();
            AutoMapper.Mapper.CreateMap<IShipProvince, ShipProvinceDisplay>();
            AutoMapper.Mapper.CreateMap<IShipRateTable, ShipRateTableDisplay>();
            AutoMapper.Mapper.CreateMap<IShipRateTier, ShipRateTierDisplay>();
            
            // warehouse
            AutoMapper.Mapper.CreateMap<IWarehouse, WarehouseDisplay>();
            AutoMapper.Mapper.CreateMap<IWarehouseCatalog, WarehouseCatalogDisplay>();            
        }
    }

}