using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello Anonymous customer
    /// </summary>
    public interface IAnonymousCustomer : IKeyEntity
    {

        /// <summary>
        /// The lastActivityDate for the Anonymous
        /// </summary>
        [DataMember]
        DateTime LastActivityDate { get; }
    }
}
