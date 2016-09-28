namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents an entity that can be deep cloned.
    /// </summary>
    public interface IDeepCloneable
    {
        /// <summary>
        /// Deep clones the entity.
        /// </summary>
        /// <returns>
        /// The cloned <see cref="object"/>.
        /// </returns>
        object DeepClone();
    }
}