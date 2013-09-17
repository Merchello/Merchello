using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="ShipMethod"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class ShipMethodMapper : MerchelloBaseMapper
    {
        public ShipMethodMapper()
        {
            BuildMap();
        }

        #region Overrides of MerchelloBaseMapper

        internal override void BuildMap()
        {
            CacheMap<ShipMethod, ShipMethodDto>(src => src.Id, dto => dto.Id);
            CacheMap<ShipMethod, ShipMethodDto>(src => src.Name, dto => dto.Name);
            CacheMap<ShipMethod, ShipMethodDto>(src => src.GatewayAlias, dto => dto.GatewayAlias);
            CacheMap<ShipMethod, ShipMethodDto>(src => src.ShipMethodTypeFieldKey, dto => dto.ShipMethodTypeFieldKey);
            CacheMap<ShipMethod, ShipMethodDto>(src => src.Surcharge, dto => dto.Surcharge);
            CacheMap<ShipMethod, ShipMethodDto>(src => src.ServiceCode, dto => dto.ServiceCode);
            CacheMap<ShipMethod, ShipMethodDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<ShipMethod, ShipMethodDto>(src => src.CreateDate, dto => dto.CreateDate);
        }

        #endregion
    }
}