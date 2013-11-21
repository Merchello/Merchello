using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public static class IProductExtensions
    {
        public static ProductDisplay ToProductDisplay(this IProduct product)
        {
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            AutoMapper.Mapper.CreateMap<IProductOption, ProductOptionDisplay>();
            AutoMapper.Mapper.CreateMap<IProductVariant, ProductVariantDisplay>();
            AutoMapper.Mapper.CreateMap<IProduct, ProductDisplay>();

            return AutoMapper.Mapper.Map<ProductDisplay>(product);
        }
    }
}
