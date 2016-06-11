﻿namespace Merchello.Web.Search
{
    using System;
    using Core;
    using Core.Services;

    using Merchello.Core.ValueConverters;

    using Umbraco.Core;

    /// <summary>
    /// Represents a CachedQueryProvider
    /// </summary>
    public class CachedQueryProvider : ICachedQueryProvider
    {
        /// <summary>
        /// A value indicating whether or not to enable any data modifiers.
        /// </summary>
        private readonly bool _enableDataModifiers;

        /// <summary>
        /// A value indicating the conversion type for detached content values.
        /// </summary>
        private readonly DetachedValuesConversionType _conversionType;

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
            : this(MerchelloContext.Current.Services, true)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedQueryProvider"/> class.
        /// </summary>
        /// <param name="serviceContext">
        /// The <see cref="IServiceContext"/>
        /// </param>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not to enable any data modifiers.
        /// </param>
        public CachedQueryProvider(IServiceContext serviceContext, bool enableDataModifiers)
            : this(serviceContext, enableDataModifiers, DetachedValuesConversionType.Db)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedQueryProvider"/> class.
        /// </summary>
        /// <param name="serviceContext">
        /// The service context.
        /// </param>
        /// <param name="enableDataModifiers">
        /// The enable data modifiers.
        /// </param>
        /// <param name="conversionType">
        /// The conversion type.
        /// </param>
        internal CachedQueryProvider(IServiceContext serviceContext, bool enableDataModifiers, DetachedValuesConversionType conversionType)
        {
            Mandate.ParameterNotNull(serviceContext, "ServiceContext is not initialized");
            _enableDataModifiers = enableDataModifiers;
            _conversionType = conversionType;
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
            _customerQuery = new Lazy<ICachedCustomerQuery>(() => new CachedCustomerQuery(serviceContext.CustomerService, _enableDataModifiers));

            if (_invoiceQuery == null)
            _invoiceQuery = new Lazy<ICachedInvoiceQuery>(() => new CachedInvoiceQuery(serviceContext.InvoiceService, _enableDataModifiers));

            if (_orderQuery == null)
            _orderQuery = new Lazy<ICachedOrderQuery>(() => new CachedOrderQuery(serviceContext.OrderService, _enableDataModifiers));

            if (_productQuery == null)
            _productQuery = new Lazy<ICachedProductQuery>(() => new CachedProductQuery(serviceContext.ProductService, _enableDataModifiers, _conversionType));
        }
    }
}