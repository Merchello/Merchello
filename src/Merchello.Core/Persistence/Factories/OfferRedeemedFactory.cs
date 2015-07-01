namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// Responsible for creating <see cref="IOfferRedeemed"/> and <see cref="OfferRedeemedDto"/>
    /// </summary>
    internal class OfferRedeemedFactory : IEntityFactory<IOfferRedeemed, OfferRedeemedDto>
    {
        /// <summary>
        /// Builds a <see cref="IOfferRedeemed"/> give a <see cref="OfferRedeemedDto"/>.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferRedeemed"/>.
        /// </returns>
        public IOfferRedeemed BuildEntity(OfferRedeemedDto dto)
        {
            var entity = new OfferRedeemed(dto.OfferCode, dto.OfferProviderKey, dto.InvoiceKey, dto.OfferSettingsKey)
                             {
                                 Key = dto.Key,                                 
                                 RedeemedDate = dto.RedeemedDate,
                                 CustomerKey = dto.CustomerKey,
                                 ExtendedData = new ExtendedDataCollection(dto.ExtendedData),
                                 CreateDate = dto.CreateDate,
                                 UpdateDate = dto.UpdateDate
                             };

            entity.ResetDirtyProperties();

            return entity;
        }

        /// <summary>
        /// Builds an <see cref="OfferRedeemedDto"/> from an <see cref="IOfferRedeemed"/> entity
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="OfferRedeemedDto"/>.
        /// </returns>
        public OfferRedeemedDto BuildDto(IOfferRedeemed entity)
        {
            return new OfferRedeemedDto()
                       {
                           Key = entity.Key, 
                           OfferSettingsKey = entity.OfferSettingsKey,
                           OfferProviderKey = entity.OfferProviderKey,
                           OfferCode = entity.OfferCode,
                           RedeemedDate = entity.RedeemedDate,
                           CustomerKey = entity.CustomerKey,
                           InvoiceKey = entity.InvoiceKey,
                           ExtendedData = entity.ExtendedData.SerializeToXml(),
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate
                       };
        }
    }
}