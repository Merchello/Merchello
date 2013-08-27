using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello Invoice Status object
    /// </summary>
    public interface IInvoiceStatus : IIdEntity
    {

        /// <summary>
        /// The name of the invoice status
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The publicName for the InvoiceStatus
        /// </summary>
        [DataMember]
        string Alias { get; set; }

        /// <summary>
        /// The reportable for the InvoiceStatus
        /// </summary>
        [DataMember]
        bool Reportable { get; set; }

        /// <summary>
        /// The active for the InvoiceStatus
        /// </summary>
        [DataMember]
        bool Active { get; set; }

        /// <summary>
        /// The actionTriggerId for the InvoiceStatus
        /// </summary>
        [DataMember]
        int ActionTriggerId { get; set; }

        /// <summary>
        /// The sortOrder for the InvoiceStatus
        /// </summary>
        [DataMember]
        int SortOrder { get; set; }
    }
}



