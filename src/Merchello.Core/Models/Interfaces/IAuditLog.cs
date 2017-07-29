namespace Merchello.Core.Models
{
    using System;
    

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents an audit log record.
    /// </summary>
    public interface IAuditLog : IHasExtendedData, IEntity
    {
        /// <summary>
        /// Gets or sets the entity key related to the audit record
        /// </summary>
        
        Guid? EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        
        Guid? EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the verbosity level.
        /// </summary>
        /// <remarks>
        /// Currently not used
        /// </remarks>
        
        int Verbosity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a record of an error.
        /// </summary>
        
        bool IsError { get; set; }
    }
}