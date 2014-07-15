namespace Merchello.Examine.DataServices
{
    /// <summary>
    /// The DataService interface.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Gets the product data service.
        /// </summary>
        IProductDataService ProductDataService { get; }

        /// <summary>
        /// Gets the invoice data service.
        /// </summary>
        IInvoiceDataService InvoiceDataService { get; }

        /// <summary>
        /// Gets the order data service.
        /// </summary>
        IOrderDataService OrderDataService { get; }

        /// <summary>
        /// Gets the customer data service.
        /// </summary>
        ICustomerDataService CustomerDataService { get; }

        /// <summary>
        /// Gets the log service.
        /// </summary>
        ILogService LogService { get; }

        /// <summary>
        /// The map path.
        /// </summary>
        /// <param name="virtualPath">
        /// The virtual path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string MapPath(string virtualPath);
    }
}