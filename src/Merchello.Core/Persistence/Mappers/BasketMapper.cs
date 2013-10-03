using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="OrderLineItem"/> to DTO mapper used to translate the properties of the public api 
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
            CacheMap<CustomerItemRegister, CustomerItemRegisterDto>(src => src.Id, dto => dto.Id);
            CacheMap<CustomerItemRegister, CustomerItemRegisterDto>(src => src.RegisterTfKey, dto => dto.RegisterTfKey);
            CacheMap<CustomerItemRegister, CustomerItemRegisterDto>(src => src.ConsumerKey, dto => dto.ConsumerKey);
            CacheMap<CustomerItemRegister, CustomerItemRegisterDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<CustomerItemRegister, CustomerItemRegisterDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
