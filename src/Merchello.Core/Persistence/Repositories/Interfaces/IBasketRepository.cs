using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the basket repository
    /// </summary>
    public interface IBasketRepository : IRepositoryQueryable<int, IBasket>
    {

    }
}
