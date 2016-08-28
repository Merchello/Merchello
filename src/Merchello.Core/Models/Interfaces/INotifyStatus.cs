using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a notification status
    /// </summary>
    public interface INotifyStatus : IEntity
    {
        /// <summary>
        /// The name of the status
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The alias of the  status
        /// </summary>
        [DataMember]
        string Alias { get; set; }

        /// <summary>
        /// True/false indicating whether or not to report on this status
        /// </summary>
        [DataMember]
        bool Reportable { get; set; }

        /// <summary>
        /// True/false indicating whether or not this  status is active
        /// </summary>
        [DataMember]
        bool Active { get; set; }

        /// <summary>
        /// The sort order of the  status
        /// </summary>
        [DataMember]
        int SortOrder { get; set; } 
    }
}