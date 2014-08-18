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
        /// The <see cref="ICachedInvoiceQuery"/>
        /// </summary>
        private Lazy<ICachedInvoiceQuery> _invoiceQuery;

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
        /// Gets the <see cref="ICachedInvoiceQuery"/>.
        /// </summary>
        public ICachedInvoiceQuery Invoice
        {
            get { return _invoiceQuery.Value; }
        }

        /// <summary>
        /// The initialize provider.
        /// </summary>
        /// <param name="serviceContext">
        /// The service context.
        /// </param>
        private void InitializeProvider(IServiceContext serviceContext)
        {
            _invoiceQuery = new Lazy<ICachedInvoiceQuery>(() => new CachedInvoiceQuery(serviceContext.InvoiceService));
        }
    }
}