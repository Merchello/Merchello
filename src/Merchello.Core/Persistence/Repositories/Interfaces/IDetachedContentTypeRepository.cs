namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the DetachedContentTypeRepository.
    /// </summary>
    public interface IDetachedContentTypeRepository : IRepositoryQueryable<Guid, IDetachedContentType>
    {         
    }
}