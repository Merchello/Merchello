namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using Models;
    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the shipiment repository
    /// </summary>
    internal interface IShipmentRepository : IRepositoryQueryable<Guid, IShipment>, IAssertsMaxDocumentNumber
    {
    }
}
