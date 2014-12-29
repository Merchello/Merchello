namespace Merchello.Web.Search
{
    /// <summary>
    /// Defines the CachedQueryProvider
    /// </summary>
    public interface ICachedQueryProvider
    {
        /// <summary>
        /// Gets the <see cref="ICachedCustomerQuery"/>
        /// </summary>
        ICachedCustomerQuery Customer { get; }

        /// <summary>
        /// Gets the <see cref="ICachedInvoiceQuery"/>.
        /// </summary>
        ICachedInvoiceQuery Invoice { get; }

        /// <summary>
        /// Gets the <see cref="ICachedOrderQuery"/>
        /// </summary>
        ICachedOrderQuery Order { get; }

        /// <summary>
        /// Gets the <see cref="ICachedProductQuery"/>
        /// </summary>
        ICachedProductQuery Product { get; }
    }
}