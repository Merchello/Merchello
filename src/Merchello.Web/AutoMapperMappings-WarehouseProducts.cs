using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Web.Models.ContentEditing;

namespace Merchello.Web
{
    /// <summary>
    /// Binds Merchello AutoMapper mappings during the Umbraco startup.
    /// </summary>
    internal static partial class AutoMapperMappings
    {
        private static void BindWarehouseAndProductMappings()
        {
            // warehouse
            AutoMapper.Mapper.CreateMap<IWarehouse, WarehouseDisplay>();
            AutoMapper.Mapper.CreateMap<IWarehouseCatalog, WarehouseCatalogDisplay>();

            // products
            AutoMapper.Mapper.CreateMap<IProduct, ProductDisplay>();
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            AutoMapper.Mapper.CreateMap<IProductOption, ProductOptionDisplay>();
            AutoMapper.Mapper.CreateMap<IProductVariant, ProductVariantDisplay>();

            AutoMapper.Mapper.CreateMap<ICatalogInventory, CatalogInventoryDisplay>();
        }
    }
}