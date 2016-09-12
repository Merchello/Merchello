namespace Merchello.Core.Models.TypeFields
{
    using System;

    /// <summary>
    /// Represents a type field.
    /// </summary>
    /// <remarks>
    /// Used for mapping enum values to <see cref="Guid"/> for database references
    /// </remarks>
    public interface ITypeField
    {
        /// <summary>
        /// Gets the unique alias of the type field.
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// Gets the descriptive name of the type field.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the type key.
        /// </summary>
        Guid TypeKey { get; }
    }
}
