using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="Basket"/> to DTO mapper used to translate the properties of the public api 
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
            CacheMap<Basket, BasketDto>(src => src.Id, dto => dto.Id);
            CacheMap<Basket, BasketDto>(src => src.BasketTypeFieldKey, dto => dto.BasketTypeFieldKey);
            CacheMap<Basket, BasketDto>(src => src.ConsumerKey, dto => dto.ConsumerKey);
            CacheMap<Basket, BasketDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<Basket, BasketDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
