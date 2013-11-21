using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public static class IProductAttributeExtensions
    {
        public static ProductAttributeDisplay ToProductDisplay(this IProductAttribute productAttribute)
        {
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();

            return AutoMapper.Mapper.Map<ProductAttributeDisplay>(productAttribute);
        }
    }
}
