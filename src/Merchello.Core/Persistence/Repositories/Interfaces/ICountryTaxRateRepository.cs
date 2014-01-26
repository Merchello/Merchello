using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the CountryTaxRateRepository
    /// </summary>
    public interface ICountryTaxRateRepository : IRepositoryQueryable<Guid, ICountryTaxRate>
    {
         
    }
}