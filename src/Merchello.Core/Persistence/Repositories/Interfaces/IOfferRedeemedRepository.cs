namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// Marker interface for OfferRedeemedRepositories.
    /// </summary>
    internal interface IOfferRedeemedRepository : IPagedRepository<IOfferRedeemed, OfferRedeemedDto>
    {
    }
}