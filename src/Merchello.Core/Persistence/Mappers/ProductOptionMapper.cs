using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal sealed class ProductOptionMapper : MerchelloBaseMapper
    {

        public ProductOptionMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<ProductOption, ProductOptionDto>(src => src.Key, dto => dto.Key);
            CacheMap<ProductOption, ProductOptionDto>(src => src.Name, dto => dto.Name);
            CacheMap<ProductOption, ProductOptionDto>(src => src.DetachedContentTypeKey, dto => dto.DetachedContentTypeKey);
            CacheMap<ProductOption, ProductOptionDto>(src => src.Required, dto => dto.Required);
            CacheMap<ProductOption, ProductOptionDto>(src => src.Shared, dto => dto.Shared);
            CacheMap<ProductOption, ProductOptionDto>(src => src.UiOption, dto => dto.UiOption);
            CacheMap<ProductOption, ProductOptionDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<ProductOption, ProductOptionDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}