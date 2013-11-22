using System.Globalization;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    public class WarehouseCountryFactory : IEntityFactory<IWarehouseCountry, WarehouseCountryDto>
    {
        public IWarehouseCountry BuildEntity(WarehouseCountryDto dto)
        {
            var entity = new WarehouseCountry(dto.WarehouseKey, dto.CountryCode)
            {
                Key = dto.Key,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate                
            };
            entity.ResetDirtyProperties();
            return entity;
        }

        public WarehouseCountryDto BuildDto(IWarehouseCountry entity)
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