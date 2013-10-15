using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="CustomerItemCacheLineItem"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class CustomerItemCacheItemMapper : MerchelloBaseMapper
    {
        public CustomerItemCacheItemMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<CustomerItemCacheLineItem, CustomerItemCacheItemDto>(src => src.Id, dto => dto.Id);
            CacheMap<CustomerItemCacheLineItem, CustomerItemCacheItemDto>(src => src.ContainerId, dto => dto.ContainerId);
            CacheMap<CustomerItemCacheLineItem, CustomerItemCacheItemDto>(src => src.LineItemTfKey, dto => dto.LineItemTfKey);
            CacheMap<CustomerItemCacheLineItem, CustomerItemCacheItemDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<CustomerItemCacheLineItem, CustomerItemCacheItemDto>(src => src.Name, dto => dto.Name);
            CacheMap<CustomerItemCacheLineItem, CustomerItemCacheItemDto>(src => src.Quantity, dto => dto.Quantity);
            CacheMap<CustomerItemCacheLineItem, CustomerItemCacheItemDto>(src => src.Amount, dto => dto.Amount);
            CacheMap<CustomerItemCacheLineItem, CustomerItemCacheItemDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<CustomerItemCacheLineItem, CustomerItemCacheItemDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
