namespace Merchello.Core.Persistence.Factories
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The entity collection factory.
    /// </summary>
    internal class EntityCollectionFactory : IEntityFactory<IEntityCollection, EntityCollectionDto>
    {
        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IEntityCollection"/>.
        /// </returns>
        public IEntityCollection BuildEntity(EntityCollectionDto dto)
        {
            var extendedData = string.IsNullOrEmpty(dto.ExtendedData) ? 
                new ExtendedDataCollection() : 
                new ExtendedDataCollection(dto.ExtendedData);

            var collection = new EntityCollection(dto.EntityTfKey, dto.ProviderKey)
                {
                    Key = dto.Key,
                    ParentKey = dto.ParentKey,
                    Name = dto.Name,
                    SortOrder = dto.SortOrder,
                    IsFilter = dto.IsFilter,
                    ExtendedData = extendedData,
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate
                };

            collection.ResetDirtyProperties();

            return collection;
        }

        /// <summary>
        /// The build dto.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionDto"/>.
        /// </returns>
        public EntityCollectionDto BuildDto(IEntityCollection entity)
        {
            var dto = new EntityCollectionDto()
                {
                    Key = entity.Key,
                    ParentKey = entity.ParentKey == Guid.Empty ? null : entity.ParentKey,
                    EntityTfKey = entity.EntityTfKey,
                    Name = entity.Name,
                    SortOrder = entity.SortOrder,
                    ProviderKey = entity.ProviderKey,
                    ExtendedData = entity.ExtendedData.SerializeToXml(),
                    IsFilter = entity.IsFilter,
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };

            return dto;
        }
    }
}