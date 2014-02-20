using System;

namespace Merchello.Core.Models.EntityBase
{
    /// <summary>
    /// Defines an entity that can be "tagged" with a versioning guid
    /// </summary>
    /// <remarks>
    /// 
    /// Used in the order fulfillment process to track the validity of a Checkout, Shipment rate qoutes, etc.
    /// 
    /// </remarks>
    public interface IVersionTaggedEntity : IEntity
    {
        Guid VersionKey { get; }
    }
}