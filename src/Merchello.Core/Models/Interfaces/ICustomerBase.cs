using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines the customer base class 
    /// </summary>
    public interface ICustomerBase : IEntity
    {
        [DataMember]
        Guid EntityKey { get; }

        /// <summary>
        /// The date the customer was last active on the site
        /// </summary>
        [DataMember]
        DateTime LastActivityDate { get; set; }

        /// <summary>
        /// True/false indicating whether or not this customer is anonymous
        /// </summary>
        [DataMember]
        bool IsAnonymous { get; }

        /// <summary>
        /// A collection to store custom/extended data for the customer
        /// </summary>
        [DataMember]
        ExtendedDataCollection ExtendedData { get; }
    }
}