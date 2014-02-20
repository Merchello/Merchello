using System.Collections.Concurrent;

namespace Merchello.Core.Persistence.Mappers
{
    internal abstract class MerchelloBaseMapper : BaseMapper
    {
        protected readonly ConcurrentDictionary<string, DtoMapModel> PropertyInfoCacheInstance = new ConcurrentDictionary<string, DtoMapModel>();

        internal abstract void BuildMap();
        

        internal override ConcurrentDictionary<string, DtoMapModel> PropertyInfoCache
        {
            get { return PropertyInfoCacheInstance; }
        }

    }
}
