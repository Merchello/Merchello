using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="Address"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class AddressMapper : MerchelloBaseMapper
    {
        
        //NOTE: its an internal class but the ctor must be public since we're using Activator.CreateInstance to create it
        // otherwise that would fail because there is no public constructor.
        public AddressMapper()
        {
            BuildMap();
        }

        #region Overrides of MerchelloBaseMapper


        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<Address, CustomerAddressDto>(src => src.Id, dto => dto.Id);
            CacheMap<Address, CustomerAddressDto>(src => src.CustomerKey, dto => dto.CustomerKey);
            CacheMap<Address, CustomerAddressDto>(src => src.Label, dto => dto.Label);
            CacheMap<Address, CustomerAddressDto>(src => src.FullName, dto => dto.FullName);
            CacheMap<Address, CustomerAddressDto>(src => src.Company, dto => dto.Company);
            CacheMap<Address, CustomerAddressDto>(src => src.AddressTypeFieldKey, dto => dto.AddressTfKey);
            CacheMap<Address, CustomerAddressDto>(src => src.Address1, dto => dto.Address1);
            CacheMap<Address, CustomerAddressDto>(src => src.Address2, dto => dto.Address2);
            CacheMap<Address, CustomerAddressDto>(src => src.Locality, dto => dto.Locality);
            CacheMap<Address, CustomerAddressDto>(src => src.Region, dto => dto.Region);
            CacheMap<Address, CustomerAddressDto>(src => src.PostalCode, dto => dto.PostalCode);
            CacheMap<Address, CustomerAddressDto>(src => src.CountryCode, dto => dto.CountryCode);
            CacheMap<Address, CustomerAddressDto>(src => src.Phone, dto => dto.Phone);
            CacheMap<Address, CustomerAddressDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<Address, CustomerAddressDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
        #endregion

    }
}
