using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    /// <summary>
    /// Represents the NotificationTriggerFactory
    /// </summary> 
    internal class NotificationTriggerFactory :  IEntityFactory<INotificationTrigger, NotificationTriggerDto>
    {
        public INotificationTrigger BuildEntity(NotificationTriggerDto dto)
        {
            var trigger = new NotificationTrigger(dto.Name, dto.Binding)
            {
                Key = dto.Key,
                EntityKey = dto.EntityKey,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            trigger.ResetDirtyProperties();

            return trigger;
        }

        public NotificationTriggerDto BuildDto(INotificationTrigger entity)
        {
            return new NotificationTriggerDto()
            {
                Key = entity.Key,
                Name = entity.Name,
                Binding = entity.Binding,
                EntityKey = entity.EntityKey,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };
        }
    }
}