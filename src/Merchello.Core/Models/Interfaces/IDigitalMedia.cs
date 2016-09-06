namespace Merchello.Core.Models.Interfaces
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    ///// TODO SR - This is just a scaffold for example.  Do whatever you need to do =)

    /// <summary>
    /// Defines DigitalMedia.
    /// </summary>
    public interface IDigitalMedia : IEntity, IHasExtendedData
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the reference to an order
        /// </summary>
        [DataMember]
        DateTime? FirstAccessed { get; set; }
    }
}