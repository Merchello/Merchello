using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="CustomerItemCache"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class CustomerItemCacheMapper : MerchelloBaseMapper
    {
        public CustomerItemCacheMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<CustomerItemCache, CustomerItemCacheDto>(src => src.Id, dto => dto.Id);
            CacheMap<CustomerItemCache, CustomerItemCacheDto>(src => src.ItemCacheTfKey, dto => dto.ItemCacheTfKey);
            CacheMap<CustomerItemCache, CustomerItemCacheDto>(src => src.CustomerKey, dto => dto.ConsumerKey);
            CacheMap<CustomerItemCache, CustomerItemCacheDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<CustomerItemCache, CustomerItemCacheDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
