namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the DigitalMediaRepository.
    /// </summary>
    public interface IDigitalMediaRepository : IRepositoryQueryable<Guid, IDigitalMedia>
    {
    }
}