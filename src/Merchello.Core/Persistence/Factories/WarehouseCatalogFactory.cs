using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class WarehouseCatalogFactory : IEntityFactory<IWarehouseCatalog, WarehouseCatalogDto>
    {
        public IWarehouseCatalog BuildEntity(WarehouseCatalogDto dto)
        {            
            var entity = new WarehouseCatalog(dto.WarehouseKey)
            {
                Key = dto.Key,
                Name = dto.Name,
                Description = dto.Description,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };
            entity.ResetDirtyProperties();
            return entity;
        }

        public WarehouseCatalogDto BuildDto(IWarehouseCatalog entity)
        {           
            return new WarehouseCatalogDto()
            {
                Key = entity.Key,
                WarehouseKey = entity.WarehouseKey,
                Name = entity.Name,
                Description = entity.Description,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

        }
    }
}