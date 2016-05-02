namespace Merchello.Core.ValueConverters.ValueOverrides
{
    using System;

    /// <summary>
    /// An attribute to decorate DetachedValue ValueConverterOverrides for resolution.
    /// </summary>
    internal class DetachedValueOverriderAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedValueOverriderAttribute"/> class.
        /// </summary>
        /// <param name="propertyEditorAlias">
        /// The property editor alias.
        /// </param>
        public DetachedValueOverriderAttribute(string propertyEditorAlias)
        {
            this.PropertyEditorAlias = propertyEditorAlias;
        }

        /// <summary>
        /// Gets the property editor alias.
        /// </summary>
        public string PropertyEditorAlias { get; private set; }
    }
}