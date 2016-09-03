namespace Merchello.Core.Models.EntityBase
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Indicates class has a Key (GUID) based Id.
    /// </summary>
    public interface IHasKeyId
    {
        /// <summary>
        /// Gets or sets the GUID based Id
        /// </summary>
        [DataMember]
        Guid Key { get; }
    }
}