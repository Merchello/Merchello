using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public static class IProductOptionExtensions
    {
        public static ProductOptionDisplay ToProductDisplay(this IProductOption productOption)
        {
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            AutoMapper.Mapper.CreateMap<IProductOption, ProductOptionDisplay>();

            return AutoMapper.Mapper.Map<ProductOptionDisplay>(productOption);
        }
    }
}
