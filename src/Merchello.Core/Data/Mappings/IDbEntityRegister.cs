namespace Merchello.Core.Data.Mappings
{
    using System.Collections.Generic;

    using Merchello.Core.Registers;

    /// <summary>
    /// Represents a register of Model Type Mapping Configurations.
    /// </summary>
    public interface IDbEntityRegister : ITypeRegister
    {
        /// <summary>
        /// Returns instantiations of resolved instance types.
        /// </summary>
        /// <returns>
        /// The collection of instantiated configuration types .
        /// </returns>
        IEnumerable<dynamic> GetInstantiations();
    }
}