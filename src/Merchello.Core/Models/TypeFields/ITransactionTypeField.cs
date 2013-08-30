namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Defines a TransactionTypeField
    /// </summary>
    public interface ITransactionTypeField : ITypeFieldMapper<TransactionType>
    {
     
        /// <summary>
        /// The Credit type
        /// </summary>
        ITypeField Credit { get; }

        /// <summary>
        /// The Debit type
        /// </summary>
        ITypeField Debit { get; }
   
    }
}