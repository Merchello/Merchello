using System.Globalization;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    public class WarehouseCountryFactory : IEntityFactory<IShipCountry, WarehouseCountryDto>
    {
        public IShipCountry BuildEntity(WarehouseCountryDto dto)
        {
            var entity = new ShipCountry(dto.WarehouseKey, dto.CountryCode)
            {
                Key = dto.Key,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate                
            };
            entity.ResetDirtyProperties();
            return entity;
        }

        public WarehouseCountryDto BuildDto(IShipCountry entity)
        {
            return new WarehouseCountryDto()
            {
                Key = entity.Key,
                WarehouseKey =  entity.WarehouseKey,
                CountryCode = entity.CountryCode,
                Name = entity.Name,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };
        }
    }
}