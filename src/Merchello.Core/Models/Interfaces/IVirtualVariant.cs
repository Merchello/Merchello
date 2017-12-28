using System;
using System.Collections.Generic;

namespace Merchello.Core.Models.Interfaces
{
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines DigitalMedia.
    /// </summary>
    public interface IVirtualVariant : IEntity
    {
        /// <summary>
        /// Gets or sets the sku.
        /// </summary>
        [DataMember]
        string Sku { get; set; }

        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        [DataMember]
        Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the first accessed date
        /// </summary>
        [DataMember]
        string Choices { get; set; }
    }
}