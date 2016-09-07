namespace Merchello.Core.Models.TypeFields
{
    using System;

    /// <summary>
    /// Represents a mapper that maps <see cref="Enum"/> to <see cref="ITypeField"/>
    /// </summary>
    /// <typeparam name="T">
    /// The type of <see cref="Enum"/> to be mapped
    /// </typeparam>
    public interface ITypeFieldMapper<T>
    {
        /// <summary>
        /// Gets the enumerated value from a type field key
        /// </summary>
        /// <param name="key">
        /// The type field key
        /// </param>
        /// <returns>
        /// The <see cref="Enum"/> value.
        /// </returns>
        T GetTypeField(Guid key);

        /// <summary>
        /// Gets the <see cref="ITypeField"/> from an enumerated value
        /// </summary>
        /// <param name="value">
        /// The enum value.
        /// </param>
        /// <returns>
        /// The <see cref="ITypeField"/>.
        /// </returns>
        ITypeField GetTypeField(T value);
    }
}