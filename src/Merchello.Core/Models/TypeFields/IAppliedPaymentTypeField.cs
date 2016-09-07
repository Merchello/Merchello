namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Represents an AppliedPayment type field
    /// </summary>
    public interface IAppliedPaymentTypeField : ITypeFieldMapper<AppliedPaymentType>
    {
        /// <summary>
        /// Gets the credit <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Credit { get; }

        /// <summary>
        /// Gets the debit <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Debit { get; }

        /// <summary>
        /// Gets the void <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Void { get; }

        /// <summary>
        /// Gets the denied <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Denied { get; }

        /// <summary>
        /// Gets the refund <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Refund { get; }
    }
}