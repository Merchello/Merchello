using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    /// <summary>
    /// Defines the notification factory
    /// </summary>
    internal class NotificationMessageFactory : IEntityFactory<INotificationMessage, NotificationMessageDto>
    {
        public INotificationMessage BuildEntity(NotificationMessageDto dto)
        {
            var notification = new NotificationMessage()
            {
                Key = dto.Key,
                Name = dto.Name,
                Description = dto.Description,
                Message = dto.Message,
                MaxLength = dto.MaxLength,
                RuleKey = dto.RuleKey,
                Recipients = dto.Recipients,
                MessageIsFilePath = dto.MessageIsFilePath,
                SendToCustomer = dto.SendToCustomer,
                Disabled = dto.Disabled,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            notification.ResetDirtyProperties();

            return notification;
        }

        public NotificationMessageDto BuildDto(INotificationMessage entity)
        {
            return new NotificationMessageDto()
            {
                Key = entity.Key,
                Name = entity.Name,
                Description = entity.Description,
                Message = entity.Message,
                MessageIsFilePath = entity.MessageIsFilePath,
                MaxLength = entity.MaxLength,
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