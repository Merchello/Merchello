using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="CustomerRegistry"/> to DTO mapper used to translate the properties of the public api 
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
            CacheMap<PurchaseLineItemContainer, CustomerRegistryItemDto>(src => src.Id, dto => dto.Id);            
            CacheMap<PurchaseLineItemContainer, CustomerRegistryItemDto>(src => src.ContainerId, dto => dto.CustomerRegistryId);
            CacheMap<PurchaseLineItemContainer, CustomerRegistryItemDto>(src => src.ParentId, dto => dto.ParentId);
            CacheMap<PurchaseLineItemContainer, CustomerRegistryItemDto>(src => src.LineItemTfKey, dto => dto.LineItemTfKey);
            CacheMap<PurchaseLineItemContainer, CustomerRegistryItemDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<PurchaseLineItemContainer, CustomerRegistryItemDto>(src => src.Name, dto => dto.Name);
            CacheMap<PurchaseLineItemContainer, CustomerRegistryItemDto>(src => src.BaseQuantity, dto => dto.BaseQuantity);
            CacheMap<PurchaseLineItemContainer, CustomerRegistryItemDto>(src => src.UnitOfMeasureMultiplier, dto => dto.UnitOfMeasureMultiplier);
            CacheMap<PurchaseLineItemContainer, CustomerRegistryItemDto>(src => src.Amount, dto => dto.Amount);
            CacheMap<PurchaseLineItemContainer, CustomerRegistryItemDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<PurchaseLineItemContainer, CustomerRegistryItemDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
