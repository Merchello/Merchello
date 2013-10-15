﻿using Merchello.Core.Models;
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
            CacheMap<OrderLineItem, OrderItemDto>(src => src.Id, dto => dto.Id);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.ContainerId, dto => dto.ContainerId);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.LineItemTfKey, dto => dto.LineItemTfKey);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.Name, dto => dto.Name);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.Quantity, dto => dto.Quantity);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.Amount, dto => dto.Amount);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.Exported, dto => dto.Exported);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<OrderLineItem, OrderItemDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}