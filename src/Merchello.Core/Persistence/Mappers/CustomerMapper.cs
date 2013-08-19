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
    /// Represents a <see cref="Customer"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    //[MapperFor(typeof(Customer))]
    //[MapperFor(typeof(ICustomer))]
    internal sealed class CustomerMapper : BaseMapper
    {
        private static readonly ConcurrentDictionary<string, DtoMapModel> PropertyInfoCacheInstance = new ConcurrentDictionary<string, DtoMapModel>();

        //NOTE: its an internal class but the ctor must be public since we're using Activator.CreateInstance to create it
        // otherwise that would fail because there is no public constructor.
        public CustomerMapper()
        {
            BuildMap();
        }

        #region Overrides of BaseMapper

        internal override ConcurrentDictionary<string, DtoMapModel> PropertyInfoCache
        {
            get { return PropertyInfoCacheInstance; }
        }

        internal override void BuildMap()
        {
            if (PropertyInfoCache.IsEmpty)
            {
                CacheMap<Customer, CustomerDto>(src => src.Key, dto => dto.Pk);
                CacheMap<Customer, CustomerDto>(src => src.FirstName, dto => dto.FirstName);
                CacheMap<Customer, CustomerDto>(src => src.LastName, dto => dto.LastName);
                CacheMap<Customer, CustomerDto>(src => src.MemberId, dto => dto.MemberId);
                CacheMap<Customer, CustomerDto>(src => src.CreateDate, dto => dto.CreateDate);
                CacheMap<Customer, CustomerDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            }
        }

        #endregion

    }
}
