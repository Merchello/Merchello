namespace Merchello.Core.Models
{
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a notification status
    /// </summary>
    public interface INotifyStatus : IEntity
    {
        /// <summary>
        /// Gets or sets the name of the status
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the alias of the  status
        /// </summary>
        [DataMember]
        string Alias { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to report on this status
        /// </summary>
        [DataMember]
        bool Reportable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this  status is active
        /// </summary>
        [DataMember]
        bool Active { get; set; }

        /// <summary>
        /// Gets or sets the sort order of the  status
        /// </summary>
        [DataMember]
        int SortOrder { get; set; } 
    }
}