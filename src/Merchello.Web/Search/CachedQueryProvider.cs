namespace Merchello.Web.Search
{
    using System;
    using Core;
    using Core.Services;

    /// <summary>
    /// Represents a CachedQueryProvider
    /// </summary>
    public class CachedQueryProvider : ICachedQueryProvider
    {
        /// <summary>
        /// The <see cref="ICachedCustomerQuery"/>.
        /// </summary>
        private Lazy<ICachedCustomerQuery> _customerQuery; 

        /// <summary>
        /// The <see cref="ICachedInvoiceQuery"/>
        /// </summary>
        private Lazy<ICachedInvoiceQuery> _invoiceQuery;

        /// <summary>
        /// The <see cref="ICachedOrderQuery"/>
        /// </summary>
        private Lazy<ICachedOrderQuery> _orderQuery;

        /// <summary>
        /// The <see cref="ICachedProductQuery"/>
        /// </summary>
        private Lazy<ICachedProductQuery> _productQuery; 

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedQueryProvider"/> class.
        /// </summary>
        public CachedQueryProvider()
            : this(MerchelloContext.Current.Services)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedQueryProvider"/> class.
        /// </summary>
        /// <param name="serviceContext">
        /// The <see cref="IServiceContext"/>
        /// </param>
        public CachedQueryProvider(IServiceContext serviceContext)
        {
            Mandate.ParameterNotNull(serviceContext, "ServiceContext is not initialized");
            InitializeProvider(serviceContext);
        }

        /// <summary>
        /// Gets the customer.
        /// </summary>
        public ICachedCustomerQuery Customer
        {
            get { return _customerQuery.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ICachedInvoiceQuery"/>.
        /// </summary>
        public ICachedInvoiceQuery Invoice
        {
            get { return _invoiceQuery.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ICachedOrderQuery"/>
        /// </summary>
        public ICachedOrderQuery Order
        {
            get { return _orderQuery.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ICachedProductQuery"/>
        /// </summary>
        public ICachedProductQuery Product
        {
            get
            {
                return _productQuery.Value;
            }
        }

        /// <summary>
        /// The initialize provider.
        /// </summary>
        /// <param name="serviceContext">
        /// The service context.
        /// </param>
        private void InitializeProvider(IServiceContext serviceContext)
        {
            if (_customerQuery == null)
            _customerQuery = new Lazy<ICachedCustomerQuery>(() => new CachedCustomerQuery(serviceContext.CustomerService));

            if (_invoiceQuery == null)
            _invoiceQuery = new Lazy<ICachedInvoiceQuery>(() => new CachedInvoiceQuery(serviceContext.InvoiceService));

            if (_orderQuery == null)
            _orderQuery = new Lazy<ICachedOrderQuery>(() => new CachedOrderQuery(serviceContext.OrderService));

            if (_productQuery == null)
            _productQuery = new Lazy<ICachedProductQuery>(() => new CachedProductQuery(serviceContext.ProductService));
        }
    }
}