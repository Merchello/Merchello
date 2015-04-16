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
                             Name = dto.Name,
                             CreateDate = dto.CreateDate,
                             UpdateDate = dto.UpdateDate
                         };

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
                              Name = entity.Name,
                              CreateDate = entity.CreateDate,
                              UpdateDate = entity.UpdateDate
                          };

            return dto;
        }
    }
}