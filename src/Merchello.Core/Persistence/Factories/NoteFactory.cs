namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
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
            var entity = new Note(dto.EntityKey, dto.EntityTfKey)
                {
                    Key = dto.Key,
                    Message = dto.Message,
                    InternalOnly = dto.InternalOnly,
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
                    InternalOnly = entity.InternalOnly,
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };
        }
    }
}