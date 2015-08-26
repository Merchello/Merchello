namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;

    /// <summary>
    /// The detached content type factory.
    /// </summary>
    internal class DetachedContentTypeFactory : IEntityFactory<IDetachedContentType, DetachedContentTypeDto>
    {
        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        public IDetachedContentType BuildEntity(DetachedContentTypeDto dto)
        {
            var content = new DetachedContentType(dto.EntityTfKey, dto.ContentTypeKey)
                {
                    Key = dto.Key,
                    Name = dto.Name,
                    Description = !dto.Description.IsNullOrWhiteSpace() ? dto.Description : string.Empty,
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate
                };

            content.ResetDirtyProperties();

            return content;
        }

        /// <summary>
        /// The build dto.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="DetachedContentTypeDto"/>.
        /// </returns>
        public DetachedContentTypeDto BuildDto(IDetachedContentType entity)
        {
            var dto = new DetachedContentTypeDto()
                {
                    Key = entity.Key,
                    ContentTypeKey = entity.ContentTypeKey,
                    EntityTfKey = entity.EntityTfKey,
                    Name = entity.Name,
                    Description = !entity.Description.IsNullOrWhiteSpace() ? entity.Description : null,
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };

            return dto;
        }
    }
}