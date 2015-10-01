namespace Merchello.Core.Persistence.Repositories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// Marker interface for the AuditLogRepository
    /// </summary>
    internal interface INoteRepository : IPagedRepository<INote, NoteDto>
    {
    }
}