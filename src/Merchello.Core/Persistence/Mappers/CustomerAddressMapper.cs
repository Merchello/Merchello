namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// Represents a <see cref="CustomerAddress"/> to DTO mapper used to translate the properties of the public API 
    /// implementation to that of the database's DTO as SQL: [tableName].[columnName].
    /// </summary>
    internal sealed class CustomerAddressMapper : MerchelloBaseMapper
    {   
        ////NOTE: its an internal class but the ctor must be public since we're using Activator.CreateInstance to create it
        //// otherwise that would fail because there is no public constructor.

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddressMapper"/> class.
        /// </summary>
        public CustomerAddressMapper()
        {
            BuildMap();
        }

        #region Overrides of MerchelloBaseMapper

        /// <summary>
        /// The build map.
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.Key, dto => dto.Key);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.CustomerKey, dto => dto.CustomerKey);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.Label, dto => dto.Label);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.FullName, dto => dto.FullName);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.Company, dto => dto.Company);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.AddressTypeFieldKey, dto => dto.AddressTfKey);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.Address1, dto => dto.Address1);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.Address2, dto => dto.Address2);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.Locality, dto => dto.Locality);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.Region, dto => dto.Region);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.PostalCode, dto => dto.PostalCode);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.CountryCode, dto => dto.CountryCode);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.Phone, dto => dto.Phone);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.IsDefault, dto => dto.IsDefault);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<CustomerAddress, CustomerAddressDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
        #endregion
    }
}
