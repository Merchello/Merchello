namespace Merchello.Core.ValueConverters.ValueCorrections
{
    /// <summary>
    /// A base class for detached value override objects.
    /// </summary>
    internal abstract class DetachedValueCorrectionBase : IDetachedValueCorrection
    {
        /// <summary>
        /// Applies the correction and returns the corrected value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public abstract object ApplyCorrection(object value);
    }
}