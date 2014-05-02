using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal sealed class NotificationTriggerMapper : MerchelloBaseMapper
    {
        public NotificationTriggerMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<NotificationTrigger, NotificationTriggerDto>(src => src.Key, dto => dto.Key);
            CacheMap<NotificationTrigger, NotificationTriggerDto>(src => src.Name, dto => dto.Name);
            CacheMap<NotificationTrigger, NotificationTriggerDto>(src => src.Binding, dto => dto.Binding);
            CacheMap<NotificationTrigger, NotificationTriggerDto>(src => src.EntityKey, dto => dto.EntityKey);
            CacheMap<NotificationTrigger, NotificationTriggerDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<NotificationTrigger, NotificationTriggerDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}