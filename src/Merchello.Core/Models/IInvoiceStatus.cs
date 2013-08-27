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
        /// The alias of the invoice status
        /// </summary>
        [DataMember]
        string Alias { get; set; }

        /// <summary>
        /// True/false indicating whether or not to report on this invoice status
        /// </summary>
        [DataMember]
        bool Reportable { get; set; }

        /// <summary>
        /// True/false indicating whether or not this invoice status is active
        /// </summary>
        [DataMember]
        bool Active { get; set; }

        /// <summary>
        /// The sort order of the invoice status
        /// </summary>
        [DataMember]
        int SortOrder { get; set; }
    }
}



