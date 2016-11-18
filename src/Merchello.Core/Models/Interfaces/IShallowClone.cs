namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents an entity that can be shallow copied.
    /// </summary>
    public interface IShallowClone
    {
        /// <summary>
        /// Performs a shallow copy of an object.
        /// </summary>
        /// <returns>
        /// The copy of the <see cref="object"/>.
        /// </returns>
        object ShallowClone();
    }
}