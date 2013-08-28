using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="Basket"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class InvoiceItemMapper : MerchelloBaseMapper
    {
        public InvoiceItemMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<InvoiceItem, InvoiceItemDto>(src => src.Id, dto => dto.Id);            
            CacheMap<InvoiceItem, InvoiceItemDto>(src => src.InvoiceId, dto => dto.InvoiceId);
            CacheMap<InvoiceItem, InvoiceItemDto>(src => src.ParentId, dto => dto.ParentId);
            CacheMap<InvoiceItem, InvoiceItemDto>(src => src.InvoiceItemTypeFieldKey, dto => dto.InvoiceItemTypeFieldKey);
            CacheMap<InvoiceItem, InvoiceItemDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<InvoiceItem, InvoiceItemDto>(src => src.Name, dto => dto.Name);
            CacheMap<InvoiceItem, InvoiceItemDto>(src => src.BaseQuantity, dto => dto.BaseQuantity);
            CacheMap<InvoiceItem, InvoiceItemDto>(src => src.UnitOfMeasureMultiplier, dto => dto.UnitOfMeasureMultiplier);
            CacheMap<InvoiceItem, InvoiceItemDto>(src => src.Amount, dto => dto.Amount);
            CacheMap<InvoiceItem, InvoiceItemDto>(src => src.Exported, dto => dto.Exported);
            CacheMap<InvoiceItem, InvoiceItemDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<InvoiceItem, InvoiceItemDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
