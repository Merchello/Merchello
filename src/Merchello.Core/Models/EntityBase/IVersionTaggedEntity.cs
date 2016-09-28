namespace Merchello.Core.Models.EntityBase
{
    using System;

    /// <summary>
    /// Represents an entity that can be "tagged" with a versioning GUID
    /// </summary>
    /// <remarks>
    /// 
    /// Used in the order fulfillment process to track the validity of a Checkout, Shipment rate quotes, etc.
    /// 
    /// </remarks>
    public interface IVersionTaggedEntity : IEntity
    {
        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        Guid VersionKey { get; set; }
    }
}