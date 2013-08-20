using System;

namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Defines a TypeFieldMapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITypeFieldMapper<T> : ICustomTypeField
    {

        /// <summary>
        /// Returns the enumerated value from the TypeKey
        /// </summary>
        /// <param name="key"><see cref="Guid"/></param>        
        T GetTypeField(Guid key);

        /// <summary>
        /// Returns a <see cref="ITypeField"/> from an enumerated value
        /// </summary>        
        ITypeField GetTypeField(T key);

    }
}