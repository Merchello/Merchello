namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The digital media factory.
    /// </summary>
    internal class DigitalMediaFactory : IEntityFactory<IDigitalMedia, DigitalMediaDto>
    {
        /// <summary>
        /// Responsible for building <see cref="IDigitalMedia"/>.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IDigitalMedia"/>.
        /// </returns>
        public IDigitalMedia BuildEntity(DigitalMediaDto dto)
        {
            var digitalMedia = new DigitalMedia
                         {
                             Key = dto.Key,
                             FirstAccessed = dto.FirstAccessed,
                             ProductVariantKey = dto.ProductVariantKey,
                             CreateDate = dto.CreateDate,
                             UpdateDate = dto.UpdateDate
                         };

            
            //Lets check for any extended data
            digitalMedia.ExtendedData = string.IsNullOrEmpty(dto.ExtendedData)
                ? new ExtendedDataCollection()
                : new ExtendedDataCollection(dto.ExtendedData);

            digitalMedia.ResetDirtyProperties();
            return digitalMedia;
        }

        /// <summary>
        /// Responsible for building <see cref="DigitalMediaDto"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="DigitalMediaDto"/>.
        /// </returns>
        public DigitalMediaDto BuildDto(IDigitalMedia entity)
        {
            var dto = new DigitalMediaDto()
                          {
                              Key = entity.Key,
                              FirstAccessed = entity.FirstAccessed,
                              ProductVariantKey = entity.ProductVariantKey,
                              CreateDate = entity.CreateDate,
                              UpdateDate = entity.UpdateDate,
                              ExtendedData = entity.ExtendedData.SerializeToXml()
                          };

            return dto;
        }
    }
}
