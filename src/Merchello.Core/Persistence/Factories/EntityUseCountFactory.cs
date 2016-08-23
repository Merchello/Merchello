namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models.Counting;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// Factory responsible for building the <see cref="EntityUseCount"/>.
    /// </summary>
    internal class EntityUseCountFactory
    {

        /// <summary>
        /// Builds the <see cref="EntityUseCount"/>.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="EntityUseCount"/>.
        /// </returns>
        public EntityUseCount Build(EntityUseCountDto dto)
        {
            return new EntityUseCount { Key = dto.Key, UseCount = dto.UseCount };
        }
    }
}