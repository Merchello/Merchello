using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class WarehouseFactory : IEntityFactory<IWarehouse, WarehouseDto>
    {
        public IWarehouse BuildEntity(WarehouseDto dto)
        {
            var catalogs = new List<IWarehouseCatalog>()
            {
                new WarehouseCatalogFactory().BuildEntity(dto.WarehouseCatalogDto)
            };

            var warehouse = new Warehouse(catalogs)
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
            var catalog = ((Warehouse) entity).WarehouseCatalogs.FirstOrDefault();
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
                CreateDate = entity.CreateDate,
                WarehouseCatalogDto = catalog != null ? new WarehouseCatalogFactory().BuildDto(catalog) : null
            };

            return dto;
        }
    }
}
