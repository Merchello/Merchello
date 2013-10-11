namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Defines an InvoiceItemTypeField
    /// </summary>
    public interface IInvoiceItemTypeField : ITypeFieldMapper<InvoiceItemType>
    {
        /// <summary>
        /// The product type
        /// </summary>
        ITypeField Item { get; }

        /// <summary>
        /// The charge type
        /// </summary>
        ITypeField Charge { get; }

        /// <summary>
        /// The shipping type
        /// </summary>
        ITypeField Shipping { get; }

        /// <summary>
        /// The tax type
        /// </summary>
        ITypeField Tax { get; }

        /// <summary>
        /// The credit type
        /// </summary>
        ITypeField Credit { get; }
    }
}