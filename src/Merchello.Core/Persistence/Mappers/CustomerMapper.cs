namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// Represents a <see cref="AnonymousCustomer"/> to DTO mapper used to translate the properties of the public API
    /// implementation to that of the database's DTO as SQL: [tableName].[columnName].
    /// </summary>
    internal sealed class CustomerMapper : MerchelloBaseMapper
    {
        ////NOTE: its an internal class but the ctor must be public since we're using Activator.CreateInstance to create it
        //// otherwise that would fail because there is no public constructor.
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerMapper"/> class.
        /// </summary>
        public CustomerMapper()
        {
            BuildMap();
        }

        #region Overrides of MerchelloBaseMapper

        /// <summary>
        /// Maps the entities
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<Customer, CustomerDto>(src => src.Key, dto => dto.Key);
            CacheMap<Customer, CustomerDto>(src => src.FirstName, dto => dto.FirstName);
            CacheMap<Customer, CustomerDto>(src => src.LastName, dto => dto.LastName);
            CacheMap<Customer, CustomerDto>(src => src.Email, dto => dto.Email);
            CacheMap<Customer, CustomerDto>(src => src.LoginName, dto => dto.LoginName);
            CacheMap<Customer, CustomerDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<Customer, CustomerDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }

        #endregion
    }
}
