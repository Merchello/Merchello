namespace Merchello.Examine.DataServices
{
    using System.Web.Hosting;
    using Core;

    /// <summary>
    /// The merchello data service.
    /// </summary>
    public class MerchelloDataService : IDataService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloDataService"/> class.
        /// </summary>
        public MerchelloDataService()
            : this(MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloDataService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public MerchelloDataService(IMerchelloContext merchelloContext)
        {
            ProductDataService = new ProductDataService(merchelloContext);
            InvoiceDataService = new InvoiceDataService(merchelloContext);
            OrderDataService = new OrderDataService(merchelloContext);
            CustomerDataService = new CustomerDataService(merchelloContext);
            LogService = new MerchelloLogService();
        }

        /// <summary>
        /// Gets the product data service.
        /// </summary>
        public IProductDataService ProductDataService { get; private set; }

        /// <summary>
        /// Gets the invoice data service.
        /// </summary>
        public IInvoiceDataService InvoiceDataService { get; private set; }

        /// <summary>
        /// Gets the order data service.
        /// </summary>
        public IOrderDataService OrderDataService { get; private set; }

        /// <summary>
        /// Gets the customer data service.
        /// </summary>
        public ICustomerDataService CustomerDataService { get; private set; }

        /// <summary>
        /// Gets the log service.
        /// </summary>
        public ILogService LogService { get; private set; }

        /// <summary>
        /// The map path.
        /// </summary>
        /// <param name="virtualPath">
        /// The virtual path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string MapPath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }
    }
}