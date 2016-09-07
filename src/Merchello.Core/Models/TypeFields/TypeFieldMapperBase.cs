namespace Merchello.Core.Models.TypeFields
{
    using System;

    /// <summary>
    /// The type field mapper base.
    /// </summary>
    public abstract class TypeFieldMapperBase
    {
        /// <summary>
        /// Gets the not found type field.
        /// </summary>
        internal static ITypeField NotFound
        {
            get
            {
                return new TypeField("NotFound", "A TypeField with the configuration specified could not be found", Guid.Empty);
            }
        }
    }
}
