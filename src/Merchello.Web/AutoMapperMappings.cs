using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Shipping.FixedRate;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Web.Models.ContentEditing;

namespace Merchello.Web
{
    /// <summary>
    /// Binds Merchello AutoMapper mappings during the Umbraco startup.
    /// </summary>
    internal static class AutoMapperMappings
    {
        public static void BindMappings()
        {
            AutoMapper.Mapper.CreateMap<ICatalogInventory, CatalogInventoryDisplay>();
            
            AutoMapper.Mapper.CreateMap<ICountry, CountryDisplay>();
            AutoMapper.Mapper.CreateMap<ITaxMethod, TaxProvinceDisplay>();
            
            // products
            AutoMapper.Mapper.CreateMap<IProduct, ProductDisplay>();
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            AutoMapper.Mapper.CreateMap<IProductOption, ProductOptionDisplay>();
            AutoMapper.Mapper.CreateMap<IProductVariant, ProductVariantDisplay>();

            AutoMapper.Mapper.CreateMap<IProvince, ProvinceDisplay>();

            // shipping            
            AutoMapper.Mapper.CreateMap<IGatewayProvider, GatewayProviderDisplay>();
            AutoMapper.Mapper.CreateMap<IGatewayResource, GatewayResourceDisplay>();
            AutoMapper.Mapper.CreateMap<IShippingGatewayProvider, ShippingGatewayProviderDisplay>();
            AutoMapper.Mapper.CreateMap<IShipCountry, ShipCountryDisplay>();
            AutoMapper.Mapper.CreateMap<IShipMethod, ShipMethodDisplay>();
            AutoMapper.Mapper.CreateMap<IFixedRateShippingGatewayMethod, RateTableShipMethodDisplay>();
            AutoMapper.Mapper.CreateMap<IShipProvince, ShipProvinceDisplay>();
            AutoMapper.Mapper.CreateMap<IShippingFixedRateTable, ShipRateTableDisplay>();
            AutoMapper.Mapper.CreateMap<IShipRateTier, ShipRateTierDisplay>();
            
            // warehouse
            AutoMapper.Mapper.CreateMap<IWarehouse, WarehouseDisplay>();
            AutoMapper.Mapper.CreateMap<IWarehouseCatalog, WarehouseCatalogDisplay>();            
        }
    }

}