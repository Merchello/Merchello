using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal sealed class OrderMapper : MerchelloBaseMapper
    {
         public OrderMapper()
         {
             BuildMap();
         }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<Order, OrderDto>(src => src.Key, dto => dto.Key);
            CacheMap<Order, OrderDto>(src => src.InvoiceKey, dto => dto.InvoiceKey);
            CacheMap<Order, OrderDto>(src => src.OrderNumber, dto => dto.OrderNumber);
            CacheMap<Order, OrderDto>(src => src.OrderNumberPrefix, dto => dto.OrderNumberPrefix);
            CacheMap<Order, OrderDto>(src => src.OrderDate, dto => dto.OrderDate);
            CacheMap<Order, OrderDto>(src => src.OrderStatusKey, dto => dto.OrderStatusKey);
            CacheMap<Order, OrderDto>(src => src.VersionKey, dto => dto.VersionKey);
            CacheMap<Order, OrderDto>(src => src.Exported, dto => dto.Exported);
            CacheMap<Order, OrderDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<Order, OrderDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}