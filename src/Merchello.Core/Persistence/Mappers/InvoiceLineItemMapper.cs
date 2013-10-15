﻿using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="InvoiceLineItem"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class InvoiceLineItemMapper : MerchelloBaseMapper
    {
        public InvoiceLineItemMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<InvoiceLineItem, InvoiceItemDto>(src => src.Id, dto => dto.Id);
            CacheMap<InvoiceLineItem, InvoiceItemDto>(src => src.ContainerId, dto => dto.ContainerId);
            CacheMap<InvoiceLineItem, InvoiceItemDto>(src => src.LineItemTfKey, dto => dto.LineItemTfKey);
            CacheMap<InvoiceLineItem, InvoiceItemDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<InvoiceLineItem, InvoiceItemDto>(src => src.Name, dto => dto.Name);
            CacheMap<InvoiceLineItem, InvoiceItemDto>(src => src.Quantity, dto => dto.Quantity);
            CacheMap<InvoiceLineItem, InvoiceItemDto>(src => src.Amount, dto => dto.Amount);
            CacheMap<InvoiceLineItem, InvoiceItemDto>(src => src.Exported, dto => dto.Exported);
            CacheMap<InvoiceLineItem, InvoiceItemDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<InvoiceLineItem, InvoiceItemDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
