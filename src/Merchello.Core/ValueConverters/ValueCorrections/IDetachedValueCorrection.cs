namespace Merchello.Core.ValueConverters.ValueCorrections
{
    /// <summary>
    /// Defines a property value converter override.
    /// </summary>
    /// <remarks>
    /// Generally used to correct discrepancies in values stored as detached content as a result of serialization to JSON 
    /// such as the multi-text string legacy property editor.
    /// 
    /// </remarks>
    internal interface IDetachedValueCorrection
    {
        /// <summary>
        /// Overrides the object value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object ApplyCorrection(object value);
    }
}