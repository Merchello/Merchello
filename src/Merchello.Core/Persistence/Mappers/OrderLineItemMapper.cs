using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="OrderLineItem"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class OrderLineItemMapper : MerchelloBaseMapper
    {
        public OrderLineItemMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<OrderLineItem, OrderItemDto>(src => src.Key, dto => dto.Key);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.ShipmentKey, dto => dto.ShipmentKey);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.ContainerKey, dto => dto.ContainerKey);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.LineItemTfKey, dto => dto.LineItemTfKey);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.Name, dto => dto.Name);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.Quantity, dto => dto.Quantity);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.Price, dto => dto.Price);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.BackOrder, dto => dto.BackOrder);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.Exported, dto => dto.Exported);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}