using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the StoreSettingRepository
    /// </summary>
    internal interface IStoreSettingRepository : IRepository<Guid, IStoreSetting>
    {
         
    }
}