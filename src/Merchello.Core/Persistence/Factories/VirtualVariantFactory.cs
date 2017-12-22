namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The digital media factory.
    /// </summary>
    internal class VirtualVariantFactory : IEntityFactory<IVirtualVariant, VirtualVariantsDto>
    {
        /// <summary>
        /// Responsible for building <see cref="IVirtualVariant"/>.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IVirtualVariant"/>.
        /// </returns>
        public IVirtualVariant BuildEntity(VirtualVariantsDto dto)
        {
            var digitalMedia = new VirtualVariant
            {
                Key = dto.Key,
                Sku = dto.Sku,
                ProductKey = dto.ProductKey,
                Choices = dto.Choices
            };

            digitalMedia.ResetDirtyProperties();

            return digitalMedia;
        }

        /// <summary>
        /// Responsible for building <see cref="VirtualVariantsDto"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="VirtualVariantsDto"/>.
        /// </returns>
        public VirtualVariantsDto BuildDto(IVirtualVariant entity)
        {
            var dto = new VirtualVariantsDto()
            {
                Key = entity.Key,
                Sku = entity.Sku,
                ProductKey = entity.ProductKey,
                Choices = entity.Choices
            };

            return dto;
        }
    }
}