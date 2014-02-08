using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="InvoiceStatus"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class InvoiceStatusMapper : MerchelloBaseMapper
    {
        public InvoiceStatusMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<InvoiceStatus, InvoiceStatusDto>(src => src.Key, dto => dto.Key);
            CacheMap<InvoiceStatus, InvoiceStatusDto>(src => src.Name, dto => dto.Name);
            CacheMap<InvoiceStatus, InvoiceStatusDto>(src => src.Alias, dto => dto.Alias);
            CacheMap<InvoiceStatus, InvoiceStatusDto>(src => src.Reportable, dto => dto.Reportable);
            CacheMap<InvoiceStatus, InvoiceStatusDto>(src => src.Active, dto => dto.Active);
            CacheMap<InvoiceStatus, InvoiceStatusDto>(src => src.SortOrder, dto => dto.SortOrder);
            CacheMap<InvoiceStatus, InvoiceStatusDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<InvoiceStatus, InvoiceStatusDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
