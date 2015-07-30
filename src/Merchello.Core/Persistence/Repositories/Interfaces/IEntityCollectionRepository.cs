namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;
    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Defines an EntityCollectionRepository.
    /// </summary>
    internal interface IEntityCollectionRepository : IRepositoryQueryable<Guid, IEntityCollection>
    {
        /// <summary>
        /// The get entity collections by product key.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        /// <remarks>
        /// Used by the StaticProductCollectionProvider
        /// </remarks>
        IEnumerable<IEntityCollection> GetEntityCollectionsByProductKey(Guid productKey);
    }
}