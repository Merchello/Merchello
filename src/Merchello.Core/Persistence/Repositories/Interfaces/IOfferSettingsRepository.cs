namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the OfferSettingsRepository.
    /// </summary>
    public interface IOfferSettingsRepository : IRepositoryQueryable<Guid, IOfferSettings>
    {         
    }
}