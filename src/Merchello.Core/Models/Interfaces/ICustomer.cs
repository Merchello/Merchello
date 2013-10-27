using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello customer
    /// </summary>
    public interface ICustomer : ICustomerBase, IIdEntity
    {
       
        /// <summary>
        /// Returns the full name of the customer
        /// </summary>
        [IgnoreDataMember]
        string FullName { get; }

        /// <summary>
        /// The first name of the customer
        /// </summary>
        [DataMember]
        string FirstName { get; set; }

        /// <summary>
        /// The last name of the customer
        /// </summary>
        [DataMember]
        string LastName { get; set; }

        /// <summary>
        /// The email address of the customer
        /// </summary>
        [DataMember]
        string Email { get; set; }

        /// <summary>
        /// The Umbraco member Id
        /// </summary>
        [DataMember]
        int? MemberId { get; set; }

        /// <summary>
        /// 
        /// The entity key of the customer
        /// </summary>
        /// <remarks>
        /// The should eventual be refactored to simply use the "Entity.Key" 
        /// </remarks>
        [DataMember]
        Guid EntityKey { get; set; }

        /// <summary>
        /// The total amount this customer has been invoiced
        /// </summary>
        [DataMember]
        decimal TotalInvoiced { get; }

        /// <summary>
        /// The total amount this customer has paid
        /// </summary>
        [DataMember]
        decimal TotalPayments { get; }

        /// <summary>
        /// The date the customer made their last payment
        /// </summary>
        [DataMember]
        DateTime? LastPaymentDate { get; }
    }
}
