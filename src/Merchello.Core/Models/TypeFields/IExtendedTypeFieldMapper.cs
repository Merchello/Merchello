namespace Merchello.Core.Models.TypeFields
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a mapper that maps <see cref="Enum"/> to <see cref="ITypeField"/> that allows
    /// for custom elements to be added via merchelloExtensibility.config
    /// </summary>
    /// <typeparam name="T">
    /// The type of <see cref="Enum"/> to be mapped
    /// </typeparam>
    public interface IExtendedTypeFieldMapper<T> : ITypeFieldMapper<T>, ICanHaveCustomTypeFields
    {
        /// <summary>
        /// Gets the collection of custom <see cref="ITypeField"/>.
        /// </summary>
        IEnumerable<ITypeField> CustomTypeFields { get; }
    }
}