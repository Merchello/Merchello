namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Defines a TransactionTypeField
    /// </summary>
    public interface IAppliedPaymentTypeField : ITypeFieldMapper<AppliedPaymentType>
    {
     
        /// <summary>
        /// The Credit type
        /// </summary>
        ITypeField Credit { get; }

        /// <summary>
        /// The Debit type
        /// </summary>
        ITypeField Debit { get; }
   
        /// <summary>
        /// The Void Type
        /// </summary>
        ITypeField Void { get; }
    }


}