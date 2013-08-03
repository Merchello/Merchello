using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Umbraco.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello customer
    /// </summary>
    public interface ICustomer : IKeyEntity
    {

        /// <summary>
        /// The Umbraco member Id
        /// </summary>
        [DataMember]
        int? MemberId { get; set; }

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
