namespace Merchello.Core.Persistence.Repositories
{
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// Marker interface for the OfferSettingsRepository.
    /// </summary>
    public interface IOfferSettingsRepository : IPagedRepository<IOfferSettings, OfferSettingsDto>
    {         
    }
}