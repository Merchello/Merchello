namespace Merchello.Core.ValueConverters.ValueOverrides
{
    /// <summary>
    /// A base class for detached value override objects.
    /// </summary>
    internal abstract class DetachedValueOverriderBase : IDetachedValueOverrider
    {
        /// <summary>
        /// The override.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public abstract object Override(object value);
    }
}