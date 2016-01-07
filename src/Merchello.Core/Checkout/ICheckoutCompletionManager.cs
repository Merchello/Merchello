namespace Merchello.Core.Checkout
{
    public interface ICheckoutCompletionManager
    {
        /// <summary>
        /// Gets or sets a prefix to be prepended to an invoice number.
        /// </summary>
        string InvoiceNumberPrefix { get; set; }
    }
}