using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    public interface ILineItemContainer : IEntity
    {
        /// <summary>
        /// A collection of <see cref="ILineItem"/>
        /// </summary>
        [DataMember]
        LineItemCollection Items { get; } 
    }
}