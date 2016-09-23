namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    public interface IReadRepository<out TEntity> : IRepository
    {
        /// <summary>
        /// Gets an Entity it's key
        /// </summary>
        TEntity Get(Guid key);

        /// <summary>
        /// Gets all entities of the spefified type
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll(params Guid[] keys);

        /// <summary>
        /// Boolean indicating whether an Entity with the specified Id exists
        /// </summary>
        bool Exists(Guid key);
    }
}