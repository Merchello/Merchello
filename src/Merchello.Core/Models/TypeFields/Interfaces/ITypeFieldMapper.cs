namespace Merchello.Core.Models.TypeFields.Interfaces
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the type field mapper.
    /// <para>
    /// This is uses to relate type fields with standard c# enumerations
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// Type of the enumeration that represents the type
    /// </typeparam>
    public interface ITypeFieldMapper<T> : ICustomTypeField
    {
        /// <summary>
        /// Gets the collection of custom type fields.
        /// </summary>
        IEnumerable<ITypeField> CustomTypeFields { get; }

        /// <summary>
        /// Returns the enumerated value from the TypeKey
        /// </summary>
        /// <param name="key">
        /// The <see cref="Guid"/>
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T GetTypeField(Guid key);

        /// <summary>
        /// Returns a <see cref="ITypeField"/> from an enumerated value
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ITypeField"/>.
        /// </returns>
        ITypeField GetTypeField(T key);
    }
}