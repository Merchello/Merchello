namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The audit log factory.
    /// </summary>
    internal class NoteFactory : IEntityFactory<INote, NoteDto>
    {
        /// <summary>
        /// Builds a <see cref="INote"/>
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="INote"/>.
        /// </returns>
        public INote BuildEntity(NoteDto dto)
        {
            var entity = new Note()
                {
                    Key = dto.Key,
                    EntityKey = dto.EntityKey,
                    EntityTfKey = dto.EntityTfKey,
                    Message = dto.Message,
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate
                };

            entity.ResetDirtyProperties();

            return entity;
        }

        /// <summary>
        /// Builds an <see cref="NoteDto"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="NoteDto"/>.
        /// </returns>
        public NoteDto BuildDto(INote entity)
        {
            return new NoteDto()
                {
                    Key = entity.Key,
                    EntityKey = entity.EntityKey,
                    EntityTfKey = entity.EntityTfKey,
                    Message = entity.Message,
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };
        }
    }
}