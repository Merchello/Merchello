using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public static class IProductVariantExtensions
    {
        public static ProductVariantDisplay ToProductDisplay(this IProductVariant productVariant)
        {
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            AutoMapper.Mapper.CreateMap<IProductVariant, ProductVariantDisplay>();

            return AutoMapper.Mapper.Map<ProductVariantDisplay>(productVariant);
        }
    }
}
