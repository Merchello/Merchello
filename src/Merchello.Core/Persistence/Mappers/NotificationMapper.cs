using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal sealed class NotificationMapper : MerchelloBaseMapper
    {
        public NotificationMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<NotificationMessage, NotificationMessageDto>(src => src.Key, dto => dto.Key);
        }
    }
}