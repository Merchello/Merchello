using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="PurchaseLineItemContainer"/> to DTO mapper used to translate the properties of the public api 
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
            CacheMap<CustomerRegistry, CustomerRegistryDto>(src => src.Id, dto => dto.Id);
            CacheMap<CustomerRegistry, CustomerRegistryDto>(src => src.CustomerRegistryTfKey, dto => dto.RegistryTfKey);
            CacheMap<CustomerRegistry, CustomerRegistryDto>(src => src.ConsumerKey, dto => dto.ConsumerKey);
            CacheMap<CustomerRegistry, CustomerRegistryDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<CustomerRegistry, CustomerRegistryDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
