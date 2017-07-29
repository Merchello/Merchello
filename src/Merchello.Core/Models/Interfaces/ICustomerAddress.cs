namespace Merchello.Core.Models
{
    using System;
    

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a customer address.
    /// </summary>
    public interface ICustomerAddress : IEntity, IShallowClone
    {
        /// <summary>
        /// Gets the customer key.
        /// </summary>
        
        Guid CustomerKey { get; }

        /// <summary>
        /// Gets or sets the descriptive label for the address
        /// </summary>
        
        string Label { get; set; }

        /// <summary>
        /// Gets or sets the full name for the address
        /// </summary>
        
        string FullName { get; set; }

        /// <summary>
        /// Gets or sets company name for the address
        /// </summary>
        
        string Company { get; set; }

        /// <summary>
        /// Gets or sets type of address indicator
        /// </summary>
        
        Guid AddressTypeFieldKey { get; set; }

        /// <summary>
        /// Gets or sets first address line
        /// </summary>
        
        string Address1 { get; set; }

        /// <summary>
        /// Gets or sets second address line
        /// </summary>
        
        string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the city or locality of the address
        /// </summary>
        
        string Locality { get; set; }

        /// <summary>
        /// Gets or sets state or province of the address
        /// </summary>
        
        string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the address
        /// </summary>
        
        string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code of the address
        /// </summary>
        
        string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the telephone number of the address
        /// </summary>
        
        string Phone { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AddressType"/> of the address
        /// </summary>
        
        AddressType AddressType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is default.
        /// </summary>
        
        bool IsDefault { get; set; }
    }
}
