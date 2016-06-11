namespace Merchello.Core.ValueConverters.ValueCorrections
{
    using System;

    /// <summary>
    /// An attribute to decorate DetachedValue to associated a correction class during resolution.
    /// </summary>
    public class DetachedValueCorrectionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedValueCorrectionAttribute"/> class.
        /// </summary>
        /// <param name="propertyEditorAlias">
        /// The property editor alias.
        /// </param>
        public DetachedValueCorrectionAttribute(string propertyEditorAlias)
        {
            this.PropertyEditorAlias = propertyEditorAlias;
        }

        /// <summary>
        /// Gets the property editor alias.
        /// </summary>
        public string PropertyEditorAlias { get; private set; }
    }
}