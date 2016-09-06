namespace Merchello.Core.Models.TypeFields
{
    using Merchello.Core.Models.TypeFields.Interfaces;

    /// <summary>
    /// Represents the type field for AppliedPayments
    /// </summary>
    public interface IAppliedPaymentTypeField : ITypeFieldMapper<AppliedPaymentType>
    {
        /// <summary>
        /// Gets the Credit type field
        /// </summary>
        ITypeField Credit { get; }

        /// <summary>
        /// Gets The Debit type field
        /// </summary>
        ITypeField Debit { get; }
   
        /// <summary>
        /// Gets the Void Type field
        /// </summary>
        ITypeField Void { get; }

        /// <summary>
        /// Gets the Denied Type field
        /// </summary>
        ITypeField Denied { get; }

        /// <summary>
        /// Gets the Refund Type field
        /// </summary>
        ITypeField Refund { get; }
    }
}