namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using Merchello.Core.Models.Interfaces;
    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Defines an EntityCollectionRepository.
    /// </summary>
    internal interface IEntityCollectionRepository : IRepositoryQueryable<Guid, IEntityCollection>
    {
    }
}