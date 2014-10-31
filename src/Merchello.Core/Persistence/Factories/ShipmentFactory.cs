namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The shipment factory.
    /// </summary>
    internal class ShipmentFactory : IEntityFactory<IShipment, ShipmentDto>
    {
        /// <summary>
        /// Builds a shipment entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IShipment"/>.
        /// </returns>
        public IShipment BuildEntity(ShipmentDto dto)
        {
            var factory = new ShipmentStatusFactory();

            var shipment = new Shipment(factory.BuildEntity(dto.ShipmentStatusDto))
            {
                Key = dto.Key,
                ShipmentNumberPrefix = dto.ShipmentNumberPrefix,
                ShipmentNumber = dto.ShipmentNumber,
                ShippedDate = dto.ShippedDate,
                FromOrganization = dto.FromOrganization,
                FromName = dto.FromName,
                FromAddress1 = dto.FromAddress1,
                FromAddress2 = dto.FromAddress2,
                FromLocality = dto.FromLocality,
                FromRegion = dto.FromRegion,
                FromPostalCode = dto.FromPostalCode,
                FromCountryCode = dto.FromCountryCode,
                FromIsCommercial = dto.FromIsCommercial,
                ToOrganization = dto.ToOrganization,
                ToName = dto.ToName,
                ToAddress1 = dto.ToAddress1,
                ToAddress2 = dto.ToAddress2,
                ToLocality = dto.ToLocality,
                ToRegion = dto.ToRegion,
                ToPostalCode = dto.ToPostalCode,
                ToCountryCode = dto.ToCountryCode,
                ToIsCommercial = dto.ToIsCommercial,
                Phone = dto.Phone,
                Email = dto.Email,
                ShipMethodKey = dto.ShipMethodKey,
                Carrier = dto.Carrier,
                TrackingCode = dto.TrackingCode,
                VersionKey = dto.VersionKey,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            shipment.ResetDirtyProperties();

            return shipment;
        }

        /// <summary>
        /// Builds a shipment dto.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="ShipmentDto"/>.
        /// </returns>
        public ShipmentDto BuildDto(IShipment entity)
        {
            var dto = new ShipmentDto()
            {
                Key = entity.Key,
                ShipmentNumberPrefix = entity.ShipmentNumberPrefix,
                ShipmentNumber = entity.ShipmentNumber,
                ShipmentStatusKey = entity.ShipmentStatusKey,
                ShippedDate = entity.ShippedDate,
                FromOrganization = entity.FromOrganization,
                FromName = entity.FromName,
                FromAddress1 = entity.FromAddress1,
                FromAddress2 = entity.FromAddress2,
                FromLocality = entity.FromLocality,
                FromRegion = entity.FromRegion,
                FromPostalCode = entity.FromPostalCode,
                FromCountryCode = entity.FromCountryCode,
                FromIsCommercial = entity.FromIsCommercial,
                ToOrganization = entity.ToOrganization,
                ToName = entity.ToName,
                ToAddress1 = entity.ToAddress1,
                ToAddress2 = entity.ToAddress2,
                ToLocality = entity.ToLocality,
                ToRegion = entity.ToRegion,
                ToPostalCode = entity.ToPostalCode,
                ToCountryCode = entity.ToCountryCode,
                ToIsCommercial = entity.ToIsCommercial,
                ShipMethodKey = entity.ShipMethodKey,
                VersionKey = entity.VersionKey,
                Phone = entity.Phone,
                Email = entity.Email,
                Carrier = entity.Carrier,
                TrackingCode = entity.TrackingCode,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
