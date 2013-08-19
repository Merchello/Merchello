using System;

namespace Merchello.Core.Models.TypeFields
{
    public interface ITypeField
    {
        /// <summary>
        /// The unique alias of the TypeField
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// The descriptive name of the TypeField
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The key associated with the TypeField
        /// </summary>
        Guid TypeKey { get; }
    }
}
