namespace Merchello.Core.Configuration
{
    /// <summary>
    /// This is used to supply optional/default values when using InnerTextConfigurationElement
    /// </summary>
    /// <typeparam name="T">
    /// The type of the inner text result
    /// </typeparam>
    /// UMBRACO_SRC
    internal class OptionalInnerTextConfigurationElement<T> : InnerTextConfigurationElement<T>
    {
        /// <summary>
        /// The wrapped configuration element.
        /// </summary>
        private readonly InnerTextConfigurationElement<T> _wrapped;

        /// <summary>
        /// A default value.
        /// </summary>
        private readonly T _defaultValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalInnerTextConfigurationElement{T}"/> class.
        /// </summary>
        /// <param name="wrapped">
        /// The <see cref="InnerTextConfigurationElement{T}"/>.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        public OptionalInnerTextConfigurationElement(InnerTextConfigurationElement<T> wrapped, T defaultValue)
        {
            this._wrapped = wrapped;
            this._defaultValue = defaultValue;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public override T Value
        {
            get { return string.IsNullOrEmpty(this._wrapped.RawValue) ? this._defaultValue : this._wrapped.Value; }
        }
    }
}