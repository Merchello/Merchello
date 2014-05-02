using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    /// <summary>
    /// Represents the NotificationMethodFactory
    /// </summary>
    internal class NotificationMethodFactory : IEntityFactory<INotificationMethod, NotificationMethodDto>
    {
        public INotificationMethod BuildEntity(NotificationMethodDto dto)
        {
            var method = new NotificationMethod(dto.ProviderKey)
            {
                Key = dto.Key,
                Name = dto.Name,
                Description = dto.Description,
                ServiceCode = dto.ServiceCode,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            method.ResetDirtyProperties();

            return method;
        }

        public NotificationMethodDto BuildDto(INotificationMethod entity)
        {
            return new NotificationMethodDto()
            {
                Key = entity.Key,
                ProviderKey = entity.ProviderKey,
                Name = entity.Name,
                Description = entity.Description,
                ServiceCode = entity.ServiceCode,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };
        }
    }
}