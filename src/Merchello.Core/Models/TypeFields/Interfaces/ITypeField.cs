namespace Merchello.Core.Models.TypeFields.Interfaces
{
    using System;

    /// <summary>
    /// Represents a type field.
    /// </summary>
    public interface ITypeField
    {
        /// <summary>
        /// Gets the unique alias of the type field
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// Gets the descriptive name of the type field
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the key associated with the type field
        /// </summary>
        Guid TypeKey { get; }
    }
}
