namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines the customer base class 
    /// </summary>
    public interface ICustomerBase : IHasExtendedData, IEntity
    {
        /// <summary>
        /// Gets the unique key for the Entity (used for ItemCaching)
        /// </summary>
        [DataMember]
        Guid EntityKey { get; }

        /// <summary>
        /// Gets or sets the date the customer was last active on the site
        /// </summary>
        [DataMember]
        DateTime LastActivityDate { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not this customer is anonymous
        /// </summary>
        [DataMember]
        bool IsAnonymous { get; }
    }
}