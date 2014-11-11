namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an invoice status.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class InvoiceStatus : NotifiedStatusBase, IInvoiceStatus
    {
    }
}