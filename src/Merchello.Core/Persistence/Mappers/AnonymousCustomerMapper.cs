using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="Merchello.Core.Models.AnonymousCustomer"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class AnonymousCustomerMapper : MerchelloBaseMapper
    {

        public AnonymousCustomerMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<AnonymousCustomer, AnonymousCustomerDto>(src => src.Key, dto => dto.Key);
            CacheMap<AnonymousCustomer, AnonymousCustomerDto>(src => src.LastActivityDate, dto => dto.LastActivityDate);
            CacheMap<AnonymousCustomer, AnonymousCustomerDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<AnonymousCustomer, AnonymousCustomerDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
