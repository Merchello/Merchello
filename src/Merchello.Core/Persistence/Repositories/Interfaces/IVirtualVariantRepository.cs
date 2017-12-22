namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the VirtualVariantRepository.
    /// </summary>
    public interface IVirtualVariantRepository : IRepositoryQueryable<Guid, IVirtualVariant>
    {
    }
}