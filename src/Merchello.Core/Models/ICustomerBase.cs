using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines the customer base class 
    /// </summary>
    public interface ICustomerBase : IKeyEntity
    {
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
    }
}