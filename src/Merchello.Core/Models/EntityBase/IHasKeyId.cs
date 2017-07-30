namespace Merchello.Core.Models.EntityBase
{
    using System;

    /// <summary>
    /// Indicates class has a Key (GUID) based Id.
    /// </summary>
    public interface IHasKeyId
    {
        /// <summary>
        /// Gets or sets the GUID based Id
        /// </summary>
        Guid Key { get; }
    }
}