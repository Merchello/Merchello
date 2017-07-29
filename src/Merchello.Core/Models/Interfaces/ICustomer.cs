namespace Merchello.Core.Models
{
    using System.Collections.Generic;
    

    /// <summary>
    /// Represents a customer
    /// </summary>
    public interface ICustomer : ICustomerBase, IHasNotes, IDeepCloneable
    {
        /// <summary>
        /// Gets the full name of the customer
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets or sets first name of the customer
        /// </summary>
        
        string FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name of the customer
        /// </summary>
        
        string LastName { get; set; }

        /// <summary>
        /// Gets or sets email address of the customer
        /// </summary>
        
        string Email { get; set; }

        /// <summary>
        /// Gets the login name.
        /// </summary>
        
        string LoginName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether tax exempt.
        /// </summary>
        
        bool TaxExempt { get; set; }

        /// <summary>
        /// Gets the addresses.
        /// </summary>
        
        IEnumerable<ICustomerAddress> Addresses { get; } 
    }
}
