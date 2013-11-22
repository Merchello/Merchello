using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class WarehouseFactory : IEntityFactory<IWarehouse, WarehouseDto>
    {
        public IWarehouse BuildEntity(WarehouseDto dto)
        {
            var warehouse = new Warehouse()
            {
                Key = dto.Key,
                Name = dto.Name,
                Address1 = dto.Address1,
                Address2 = dto.Address2,
                Locality = dto.Locality,
                Region = dto.Region,
                PostalCode = dto.PostalCode,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            warehouse.ResetDirtyProperties();

            return warehouse;
        }

        public WarehouseDto BuildDto(IWarehouse entity)
        {
            var dto = new WarehouseDto()
            {
                Key = entity.Key,
                Name = entity.Name,
                Address1 = entity.Address1,
                Address2 = entity.Address2,
                Locality = entity.Locality,
                Region = entity.Region,
                PostalCode = entity.PostalCode,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate                
            };

            return dto;
        }
    }
}
