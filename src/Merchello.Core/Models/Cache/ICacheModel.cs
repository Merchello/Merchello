namespace Merchello.Core.Models.Cache
{
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a model intended to be used in repository caching.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The model or entity to be cached
    /// </typeparam>
    public interface ICacheModel<out TEntity>
        where TEntity : class, IEntity, IDeepCloneable
    {
        /// <summary>
        /// Gets the model or entity cached.
        /// </summary>
        TEntity Model { get; }

        /// <summary>
        /// Wrapper to call the models <see cref="IDeepCloneable"/> method.
        /// </summary>
        /// <returns>
        /// A deep clone of the <see cref="TEntity"/>.
        /// </returns>
        TEntity InnerDeepClone();
    }
}