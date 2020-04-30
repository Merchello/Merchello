namespace Merchello.Web.Search
{
    using System;
    using Core;
    using Core.Services;

    using Merchello.Core.Models;
    using Merchello.Core.ValueConverters;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core;

    /// <summary>
    /// Represents a CachedQueryProvider
    /// </summary>
    public class CachedQueryProvider : ICachedQueryProvider
    {
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
        /// A value indicating whether or not to enable any data modifiers.
        /// </summary>
        private bool _enableDataModifiers;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedQueryProvider"/> class.
        /// </summary>
        public CachedQueryProvider()
            : this(MerchelloContext.Current, true)
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
        [Obsolete("Use the constructor that passes the MerchelloContext")]
        public CachedQueryProvider(IServiceContext serviceContext, bool enableDataModifiers)
            : this(MerchelloContext.Current, enableDataModifiers, DetachedValuesConversionType.Db)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedQueryProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="enableDataModifiers">
        /// The enable data modifiers.
        /// </param>
        public CachedQueryProvider(IMerchelloContext merchelloContext, bool enableDataModifiers)
            : this(merchelloContext, enableDataModifiers, DetachedValuesConversionType.Db)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedQueryProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchelloContext context.
        /// </param>
        /// <param name="enableDataModifiers">
        /// The enable data modifiers.
        /// </param>
        /// <param name="conversionType">
        /// The conversion type.
        /// </param>
        internal CachedQueryProvider(IMerchelloContext merchelloContext, bool enableDataModifiers, DetachedValuesConversionType conversionType)
        {
            Ensure.ParameterNotNull(merchelloContext, "MerchelloContext is not initialized");
            this.DataModifiersEnabled = enableDataModifiers;
            _conversionType = conversionType;
            InitializeProvider(merchelloContext);
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

        internal bool DataModifiersEnabled { get; private set; } 

        /// <summary>
        /// Allows for externally setting the enable data modifiers property.
        /// </summary>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        internal void SetDataModifiers(bool enabled = true)
        {
            this.DataModifiersEnabled = enabled;

            // These are currently not used so there is no reason to instantiate the Lazy if we don't need
            // ((CachedQueryBase)_customerQuery.Value).EnableDataModifiers = enabled;
            // ((CachedQueryBase)_invoiceQuery.Value).EnableDataModifiers = enabled;
            // ((CachedQueryBase)_orderQuery.Value).EnableDataModifiers = enabled;
            ((CachedQueryBase)_productQuery.Value).EnableDataModifiers = enabled;
        }

        /// <summary>
        /// The initialize provider.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        private void InitializeProvider(IMerchelloContext merchelloContext)
        {
            if (_customerQuery == null)
            _customerQuery = new Lazy<ICachedCustomerQuery>(() => new CachedCustomerQuery(merchelloContext, DataModifiersEnabled));

            if (_invoiceQuery == null)
            _invoiceQuery = new Lazy<ICachedInvoiceQuery>(() => new CachedInvoiceQuery(merchelloContext, DataModifiersEnabled));

            if (_orderQuery == null)
            _orderQuery = new Lazy<ICachedOrderQuery>(() => new CachedOrderQuery(merchelloContext, DataModifiersEnabled));

            if (_productQuery == null)
            _productQuery = new Lazy<ICachedProductQuery>(() => new CachedProductQuery(merchelloContext, DataModifiersEnabled, _conversionType));
        }
    }
}