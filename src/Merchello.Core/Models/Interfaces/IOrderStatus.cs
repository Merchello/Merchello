using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    public interface IOrderStatus : IEntity
    {
        /// <summary>
        /// The name of the order status
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The alias of the order status
        /// </summary>
        [DataMember]
        string Alias { get; set; }

        /// <summary>
        /// True/false indicating whether or not to report on this order status
        /// </summary>
        [DataMember]
        bool Reportable { get; set; }

        /// <summary>
        /// True/false indicating whether or not this order status is active
        /// </summary>
        [DataMember]
        bool Active { get; set; }

        /// <summary>
        /// The sort order of the order status
        /// </summary>
        [DataMember]
        int SortOrder { get; set; } 
    }
}