namespace Merchello.Web
{
    using Core.Models;
    using Core.Models.Interfaces;
    using Models.ContentEditing;

    /// <summary>
    /// Binds Merchello AutoMapper mappings during the Umbraco startup.
    /// </summary>
    internal static partial class AutoMapperMappings
    {
        /// <summary>
        /// Creates warehouse and product mappings.
        /// </summary>
        private static void CreateWarehouseAndProductMappings()
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