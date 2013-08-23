using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Persistence.Mappers
{
    internal abstract class MerchelloBaseMapper : BaseMapper
    {
        protected static readonly ConcurrentDictionary<string, DtoMapModel> PropertyInfoCacheInstance = new ConcurrentDictionary<string, DtoMapModel>();


        internal abstract void BuildMap();
        

        internal override ConcurrentDictionary<string, DtoMapModel> PropertyInfoCache
        {
            get { return PropertyInfoCacheInstance; }
        }

    }
}
