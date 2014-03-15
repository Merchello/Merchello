using System.Data.SqlClient;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class OrderStatusFactory : IEntityFactory<IOrderStatus, OrderStatusDto>
    {
        public IOrderStatus BuildEntity(OrderStatusDto dto)
        {
            var status = new OrderStatus()
                {
                    Key = dto.Key,
                    Name = dto.Name,
                    Alias = dto.Alias,
                    Reportable = dto.Reportable,
                    Active = dto.Active,
                    SortOrder = dto.SortOrder,
                    UpdateDate = dto.UpdateDate,
                    CreateDate = dto.CreateDate
                };

            status.ResetDirtyProperties();

            return status;
        }

        public OrderStatusDto BuildDto(IOrderStatus entity)
        {
            return new OrderStatusDto()
            {
                Key = entity.Key,
                Name = entity.Name,
                Alias = entity.Alias,
                Reportable = entity.Reportable,
                Active = entity.Active,
                SortOrder = entity.SortOrder,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };
        }
    }
}