using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a purchase item
    /// </summary>
    public interface IOrderLineItem : ILineItem
    {
        /// <summary>
        /// The unitOfMeasureMultiplier for the basket item
        /// </summary>
        [DataMember]
        int UnitOfMeasureMultiplier { get; set; }

    }
}
