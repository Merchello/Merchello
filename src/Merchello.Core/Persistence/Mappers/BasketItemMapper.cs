using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="CustomerItemRegister"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class BasketItemMapper : MerchelloBaseMapper
    {
        public BasketItemMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<OrderLineItem, CustomerItemRegisterItemDto>(src => src.Id, dto => dto.Id);            
            CacheMap<OrderLineItem, CustomerItemRegisterItemDto>(src => src.ContainerId, dto => dto.CustomerItemRegisterId);
            CacheMap<OrderLineItem, CustomerItemRegisterItemDto>(src => src.ParentId, dto => dto.ParentId);
            CacheMap<OrderLineItem, CustomerItemRegisterItemDto>(src => src.LineItemTfKey, dto => dto.LineItemTfKey);
            CacheMap<OrderLineItem, CustomerItemRegisterItemDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<OrderLineItem, CustomerItemRegisterItemDto>(src => src.Name, dto => dto.Name);
            CacheMap<OrderLineItem, CustomerItemRegisterItemDto>(src => src.BaseQuantity, dto => dto.BaseQuantity);
            CacheMap<OrderLineItem, CustomerItemRegisterItemDto>(src => src.UnitOfMeasureMultiplier, dto => dto.UnitOfMeasureMultiplier);
            CacheMap<OrderLineItem, CustomerItemRegisterItemDto>(src => src.Amount, dto => dto.Amount);
            CacheMap<OrderLineItem, CustomerItemRegisterItemDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<OrderLineItem, CustomerItemRegisterItemDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
