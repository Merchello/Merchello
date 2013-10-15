using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="ItemCacheLineItem"/> to DTO mapper used to translate the properties of the public api 
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
            CacheMap<ItemCacheLineItem, ItemCacheItemDto>(src => src.Id, dto => dto.Id);
            CacheMap<ItemCacheLineItem, ItemCacheItemDto>(src => src.ContainerId, dto => dto.ContainerId);
            CacheMap<ItemCacheLineItem, ItemCacheItemDto>(src => src.LineItemTfKey, dto => dto.LineItemTfKey);
            CacheMap<ItemCacheLineItem, ItemCacheItemDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<ItemCacheLineItem, ItemCacheItemDto>(src => src.Name, dto => dto.Name);
            CacheMap<ItemCacheLineItem, ItemCacheItemDto>(src => src.Quantity, dto => dto.Quantity);
            CacheMap<ItemCacheLineItem, ItemCacheItemDto>(src => src.Amount, dto => dto.Amount);
            CacheMap<ItemCacheLineItem, ItemCacheItemDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<ItemCacheLineItem, ItemCacheItemDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
