namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The audit log mapper.
    /// </summary>
    internal sealed class NoteMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogMapper"/> class.
        /// </summary>
        public NoteMapper()
        {
            BuildMap();
        }

        /// <summary>
        /// The build map.
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<Note, NoteDto>(src => src.Key, dto => dto.Key);
            CacheMap<Note, NoteDto>(src => src.EntityKey, dto => dto.EntityKey);
            CacheMap<Note, NoteDto>(src => src.EntityTfKey, dto => dto.EntityTfKey);
            CacheMap<Note, NoteDto>(src => src.Author, dto => dto.Author);
            CacheMap<Note, NoteDto>(src => src.InternalOnly, dto => dto.InternalOnly);
            CacheMap<Note, NoteDto>(src => src.Message, dto => dto.Message);
            CacheMap<Note, NoteDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<Note, NoteDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}