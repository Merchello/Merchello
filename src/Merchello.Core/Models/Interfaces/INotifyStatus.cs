namespace Merchello.Core.Models
{
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a status
    /// </summary>
    public interface INotifyStatus : IEntity
    {
        /// <summary>
        /// Gets or sets the name of the status
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the alias of the  status
        /// </summary>
        string Alias { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to report on this status
        /// </summary>
        bool Reportable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this  status is active
        /// </summary>
        bool Active { get; set; }

        /// <summary>
        /// Gets or sets the sort order of the  status
        /// </summary>
        int SortOrder { get; set; } 
    }
}