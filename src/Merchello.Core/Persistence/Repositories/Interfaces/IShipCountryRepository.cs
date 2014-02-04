using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for teh ship country repository
    /// </summary>
    internal interface IShipCountryRepository : IRepositoryQueryable<Guid, IShipCountry>
    {
        bool Exists(Guid catalogKey, string countryCode);
    }
}