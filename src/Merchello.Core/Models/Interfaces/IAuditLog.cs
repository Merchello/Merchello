namespace Merchello.Core.Models.Interfaces
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines an Audit record.
    /// </summary>
    public interface IAuditLog : IHasExtendedData, IEntity
    {
        /// <summary>
        /// Gets or sets the entity key related to the audit record
        /// </summary>
        [DataMember]
        Guid? EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        [DataMember]
        Guid? EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [DataMember]
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the verbosity level.
        /// </summary>
        /// <remarks>
        /// Currently not used
        /// </remarks>
        [DataMember]
        int Verbosity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a record of an error.
        /// </summary>
        [DataMember]
        bool IsError { get; set; }
    }
}