using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="CustomerItemCache"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class CustomerItemRegisterItemMapper : MerchelloBaseMapper
    {
        public CustomerItemRegisterItemMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<OrderLineItem, CustomerItemCacheItemDto>(src => src.Id, dto => dto.Id);            
            CacheMap<OrderLineItem, CustomerItemCacheItemDto>(src => src.ContainerId, dto => dto.ContainerId);
            CacheMap<OrderLineItem, CustomerItemCacheItemDto>(src => src.LineItemTfKey, dto => dto.LineItemTfKey);
            CacheMap<OrderLineItem, CustomerItemCacheItemDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<OrderLineItem, CustomerItemCacheItemDto>(src => src.Name, dto => dto.Name);
            CacheMap<OrderLineItem, CustomerItemCacheItemDto>(src => src.Quantity, dto => dto.Quantity);
            CacheMap<OrderLineItem, CustomerItemCacheItemDto>(src => src.Amount, dto => dto.Amount);
            CacheMap<OrderLineItem, CustomerItemCacheItemDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<OrderLineItem, CustomerItemCacheItemDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
