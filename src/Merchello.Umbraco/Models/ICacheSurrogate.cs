namespace Merchello.Umbraco.Models
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Cache;
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// A cache surrogate to take advantage of Umbraco's "new cache" cloning mechanisms.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the Merchello <see cref="IEntity"/>
    /// </typeparam>
    /// <remarks>
    /// We have to wrap Merchello's entities in a proxy wrapper so that they can implement Umbraco's
    /// IDeepCloneable interface and not have to have a full dependency on the Umbraco Core.
    /// </remarks>
    public interface ICacheSurrogate<out TEntity> : ICacheModel<TEntity>, global::Umbraco.Core.Models.IDeepCloneable
        where TEntity : class, IEntity, IDeepCloneable
    {
    }
}