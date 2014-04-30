using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    /// <summary>
    /// Defines the notification factory
    /// </summary>
    internal class NotificationFactory : IEntityFactory<INotification, NotificationDto>
    {
        public INotification BuildEntity(NotificationDto dto)
        {
            var notification = new Notification()
            {
                Key = dto.Key,
                Name = dto.Name,
                Description = dto.Description,
                Src = dto.Src,
                RuleKey = dto.RuleKey,
                Recipients = dto.Recipients,
                SendToCustomer = dto.SendToCustomer,
                Disabled = dto.Disabled,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            notification.ResetDirtyProperties();

            return notification;
        }

        public NotificationDto BuildDto(INotification entity)
        {
            return new NotificationDto()
            {
                Key = entity.Key,
                Name = entity.Name,
                Description = entity.Description,
                Src = entity.Src,
                RuleKey = entity.RuleKey,
                Recipients = entity.Recipients,
                SendToCustomer = entity.SendToCustomer,
                Disabled = entity.Disabled,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };
        }
    }
}