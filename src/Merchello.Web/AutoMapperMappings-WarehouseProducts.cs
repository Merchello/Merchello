namespace Merchello.Web
{
    using System.Linq.Expressions;
    using Core.Models;
    using Core.Models.Interfaces;

    using Merchello.Core.Models.DetachedContent;
    using Merchello.Web.Models.ContentEditing.Content;
    using Merchello.Web.Models.MapperResolvers.DetachedContent;
    using Merchello.Web.Models.MapperResolvers.Offers;
    using Merchello.Web.Models.MapperResolvers.ProductOptions;

    using Models.ContentEditing;
    using Models.MapperResolvers;

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
            AutoMapper.Mapper.CreateMap<IWarehouseCatalog, WarehouseCatalogDisplay>()
                 .ForMember(dest => dest.IsDefault, opt => opt.ResolveUsing<WarehouseCatalogIsDefaultResolver>().ConstructedBy(() => new WarehouseCatalogIsDefaultResolver()));

            // products
            AutoMapper.Mapper.CreateMap<IProduct, ProductDisplay>();
            AutoMapper.Mapper.CreateMap<IProductVariant, ProductVariantDisplay>();

            AutoMapper.Mapper.CreateMap<ProductDisplay, ProductVariantDisplay>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(x => x.ProductVariantKey))
                .ForMember(dest => dest.ProductKey, opt => opt.MapFrom(x => x.Key));
           
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>()
                .ForMember(
                    dest => dest.DetachedDataValues,
                    opt =>
                    opt.ResolveUsing<ProductAttributeDetachedDataValuesResolver>().ConstructedBy(() => new ProductAttributeDetachedDataValuesResolver()));

            AutoMapper.Mapper.CreateMap<IProductOption, ProductOptionDisplay>()
                .ForMember(
                    dest => dest.ShareCount,
                    opt =>
                    opt.ResolveUsing<ProductOptionSharedCountResolver>()
                        .ConstructedBy(() => new ProductOptionSharedCountResolver()));

            AutoMapper.Mapper.CreateMap<ICatalogInventory, CatalogInventoryDisplay>();            
        }
    }
}