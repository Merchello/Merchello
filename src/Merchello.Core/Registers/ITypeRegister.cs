namespace Merchello.Core.Registers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a register for know types.
    /// </summary>
    public interface ITypeRegister
    {
        /// <summary>
        /// Gets the resolved instance types.
        /// </summary>
        IEnumerable<Type> InstanceTypes { get; }
    }
}