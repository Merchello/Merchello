using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the TaxMethodRepository
    /// </summary>
    public interface ITaxMethodRepository : IRepositoryQueryable<Guid, ITaxMethod>
    {
    }
}