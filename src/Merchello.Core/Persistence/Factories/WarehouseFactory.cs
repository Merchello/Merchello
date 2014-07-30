namespace Merchello.Core.Persistence.Factories
{
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The warehouse factory.
    /// </summary>
    internal class WarehouseFactory : IEntityFactory<IWarehouse, WarehouseDto>
    {
        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IWarehouse"/>.
        /// </returns>
        public IWarehouse BuildEntity(WarehouseDto dto)
        {
            return BuildEntity(dto, new List<IWarehouseCatalog>());
        }

        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <param name="warehouseCatalogs">
        /// The warehouse Catalogs.
        /// </param>
        /// <returns>
        /// The <see cref="IWarehouse"/>.
        /// </returns>
        public IWarehouse BuildEntity(WarehouseDto dto, IEnumerable<IWarehouseCatalog> warehouseCatalogs)
        {
            var warehouse = new Warehouse(warehouseCatalogs)
            {
                Key = dto.Key,
                Name = dto.Name,
                Address1 = dto.Address1,
                Address2 = dto.Address2,
                Locality = dto.Locality,
                Region = dto.Region,
                PostalCode = dto.PostalCode,
                CountryCode = dto.CountryCode,
                Email = dto.Email,
                IsDefault = dto.IsDefault,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            warehouse.ResetDirtyProperties();

            return warehouse;
        }

        /// <summary>
        /// The build dto.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="WarehouseDto"/>.
        /// </returns>
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
                CountryCode = entity.CountryCode,
                Email = entity.Email,
                IsDefault = entity.IsDefault,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
