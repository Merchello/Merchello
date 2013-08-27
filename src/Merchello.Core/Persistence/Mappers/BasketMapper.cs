using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="Basket"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class BasketMapper : MerchelloBaseMapper
    {
        public BasketMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<BasketItem, BasketItemDto>(src => src.Id, dto => dto.Id);            
            CacheMap<BasketItem, BasketItemDto>(src => src.BasketId, dto => dto.BasketId);
            CacheMap<BasketItem, BasketItemDto>(src => src.ParentId, dto => dto.ParentId);
            CacheMap<BasketItem, BasketItemDto>(src => src.InvoiceItemTypeFieldKey, dto => dto.InvoiceItemTypeFieldKey);
            CacheMap<BasketItem, BasketItemDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<BasketItem, BasketItemDto>(src => src.Name, dto => dto.Name);
            CacheMap<BasketItem, BasketItemDto>(src => src.BaseQuantity, dto => dto.BaseQuantity);
            CacheMap<BasketItem, BasketItemDto>(src => src.UnitOfMeasureMultiplier, dto => dto.UnitOfMeasureMultiplier);
            CacheMap<BasketItem, BasketItemDto>(src => src.Amount, dto => dto.Amount);
            CacheMap<BasketItem, BasketItemDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<BasketItem, BasketItemDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
