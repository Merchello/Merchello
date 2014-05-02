using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the NotificationMethodRespository
    /// </summary>
    public interface INotificationMethodRepository : IRepositoryQueryable<Guid, INotificationMethod>
    { }
}