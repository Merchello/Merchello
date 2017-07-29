namespace Merchello.Core.Models
{
    using System;
    

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a base customer implementation. 
    /// </summary>
    public interface ICustomerBase : IHasExtendedData, IEntity
    {
        /// <summary>
        /// Gets or sets the date the customer was last active on the site
        /// </summary>
        
        DateTime LastActivityDate { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not this customer is anonymous
        /// </summary>
        
        bool IsAnonymous { get; }
    }
}