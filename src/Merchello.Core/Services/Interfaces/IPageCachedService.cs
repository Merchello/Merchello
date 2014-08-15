namespace Merchello.Core.Services
{
    using System;
    using Models.EntityBase;
    using Umbraco.Core.Services;

    /// <summary>
    /// Marks paged cache services.
    /// </summary>
    /// <typeparam name="TEnitity">
    /// The type of entity
    /// </typeparam>
    public interface IPageCachedService<out TEnitity> : IService
        where TEnitity : class, IEntity
    {
        /// <summary>
        /// Gets an entity by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="TEnitity"/>.
        /// </returns>
        TEnitity GetByKey(Guid key);
    }
}