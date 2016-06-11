namespace Merchello.Core.Models.EntityBase
{
    /// <summary>
    /// An interface that defines the object is tracking property changes and if it is Dirty
    /// </summary>
    public interface ITracksDirty
    {
        /// <summary>
        /// The is dirty.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsDirty();

        /// <summary>
        /// The is property dirty.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsPropertyDirty(string propertyName);

        /// <summary>
        /// The reset dirty properties.
        /// </summary>
        void ResetDirtyProperties();
    }
}
