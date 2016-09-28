namespace Merchello.Core.Persistence.Factories
{
    /// <summary>
    /// Represents an entity factory.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity.
    /// </typeparam>
    /// <typeparam name="TDto">
    /// The type of DTO
    /// </typeparam>
    internal interface IEntityFactory<TEntity, TDto> 
        where TEntity : class
        where TDto : class
    {
        /// <summary>
        /// Builds an entity from a DTO.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        TEntity BuildEntity(TDto dto);

        /// <summary>
        /// Builds a DTO from an entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="TDto"/>.
        /// </returns>
        TDto BuildDto(TEntity entity);
    }
}
