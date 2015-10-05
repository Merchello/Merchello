namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core;
    using Core.Services;
    
    using global::Examine.SearchCriteria;

    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Validation;

    using Models.ContentEditing;
    using Search;
    using Umbraco.Core;

    /// <summary>
    /// A helper class that provides many useful methods and functionality for using Merchello in templates
    /// </summary> 
    /// <remarks>
    /// TODO Refactor the MerchelloHelper in version 2 to take MerchelloContext rather than the ServiceContext
    /// </remarks>
    public class MerchelloHelper
    {
        /// <summary>
        /// The <see cref="ICachedQueryProvider"/>
        /// </summary>
        private readonly Lazy<ICachedQueryProvider> _queryProvider;

        /// <summary>
        /// The <see cref="IValidationHelper"/>.
        /// </summary>
        private readonly Lazy<IValidationHelper> _validationHelper;

        /// <summary>
        /// A value indicating whether or not data modifiers are enabled.
        /// </summary>
        private readonly bool _enableDataModifiers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloHelper"/> class.
        /// </summary>
        public MerchelloHelper()
            : this(true)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloHelper"/> class.
        /// </summary>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not to enable data modifiers
        /// </param>
        public MerchelloHelper(bool enableDataModifiers)
            : this(MerchelloContext.Current.Services, enableDataModifiers)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloHelper"/> class.
        /// </summary>
        /// <param name="serviceContext">
        /// The service context.
        /// </param>
        /// <remarks>
        /// This constructor needs to be removed eventually as it assumes that we want to 
        /// enable the data modifiers which might be unexpected for some implementations.  It's 
        /// need here to prevent a breaking change in version 1.9.1
        /// </remarks>
        public MerchelloHelper(IServiceContext serviceContext)
            : this(serviceContext, true)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloHelper"/> class.
        /// </summary>
        /// <param name="serviceContext">
        /// The service context.
        /// </param>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not to enable data modifiers
        /// </param>
        public MerchelloHelper(IServiceContext serviceContext, bool enableDataModifiers)
        {
            Mandate.ParameterNotNull(serviceContext, "ServiceContext cannot be null");

            _enableDataModifiers = enableDataModifiers;
            _queryProvider = new Lazy<ICachedQueryProvider>(() => new CachedQueryProvider(serviceContext, _enableDataModifiers));
            _validationHelper = new Lazy<IValidationHelper>(() => new ValidationHelper());
        }

        /// <summary>
        /// Gets the <see cref="ICachedQueryProvider"/>
        /// </summary>
        public ICachedQueryProvider Query
        {
            get { return _queryProvider.Value; }
        }

        /// <summary>
        /// Gets the <see cref="IValidationHelper"/>.
        /// </summary>
        public IValidationHelper Validation
        {
            get
            {
                return _validationHelper.Value;
            }
        }

        /// <summary>
        /// Gets a <see cref="IProductContent"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public IProductContent TypedProductContent(string key)
        {
            return this.TypedProductContent(new Guid(key));
        }

        /// <summary>
        /// Gets a <see cref="IProductContent"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public IProductContent TypedProductContent(Guid key)
        {
            var display = Query.Product.GetByKey(key);
            return display == null ? null : 
                display.AsProductContent();
        }

        /// <summary>
        /// Gets a <see cref="IProductContent"/> by it's slug.
        /// </summary>
        /// <param name="slug">
        /// The slug.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public IProductContent TypedProductContentBySlug(string slug)
        {
            var display = Query.Product.GetBySlug(slug);
            return display == null ? null : 
                display.AsProductContent();
        }

        /// <summary>
        /// Gets a <see cref="IProductContent"/> by it's SKU.
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public IProductContent TypeProductContentBySku(string sku)
        {
            var display = Query.Product.GetBySku(sku);
            return display == null ? null :
                display.AsProductContent();
        }

        /// <summary>
        /// The typed product content from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public IEnumerable<IProductContent> TypedProductContentFromCollection(Guid collectionKey)
        {
            var products = Query.Product.GetFromCollection(collectionKey, 1, long.MaxValue).Items
                    .Select(x => (ProductDisplay)x)
                    .Where(x => x.Available && x.DetachedContents.Any(y => y.CanBeRendered));

            var factory = new ProductContentFactory();
            return products.Select(factory.BuildContent);
        }
        
            #region Product

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> from the Merchello Product index.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        [Obsolete("Use MerchelloHelper.Query.Product.GetByKey")]
        public ProductDisplay Product(string key)
        {
            return Product(new Guid(key));
        }

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> from the Merchello Product index.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        [Obsolete("Use MerchelloHelper.Query.Product.GetByKey")]
        public ProductDisplay Product(Guid key)
        {
            return Query.Product.GetByKey(key);
        }


        /// <summary>
        /// Retrieves a <see cref="ProductVariantDisplay"/> from the Merchello Product index.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        [Obsolete("Use MerchelloHelper.Query.Product.GetProductVariantByKey")]
        public ProductVariantDisplay ProductVariant(string key)
        {
            return ProductVariant(new Guid(key));
        }

        /// <summary>
        /// Retrieves a <see cref="ProductVariantDisplay"/> from the Merchello Product index.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        [Obsolete("Use MerchelloHelper.Query.Product.GetProductVariantByKey")]
        public ProductVariantDisplay ProductVariant(Guid key)
        {
            return Query.Product.GetProductVariantByKey(key);
        }


        /// <summary>
        /// Get a product variant from a product by it's collection of attributes
        /// </summary>
        /// <param name="productKey">The product key</param>
        /// <param name="attributeKeys">The option choices (attributeKeys)</param>
        /// <returns>The <see cref="ProductVariantDisplay"/></returns>
        public ProductVariantDisplay GetProductVariantWithAttributes(Guid productKey, Guid[] attributeKeys)
        {
            var product = Query.Product.GetByKey(productKey);
            return product.ProductVariants.FirstOrDefault(x => x.Attributes.Count() == attributeKeys.Count() && attributeKeys.All(key => x.Attributes.FirstOrDefault(att => att.Key == key) != null));
        }

        /// <summary>
        /// Gets a list of valid variants based on partial attribute selection
        /// </summary>
        /// <param name="productKey">The product key</param>
        /// <param name="attributeKeys">The selected option choices</param>
        /// <returns>A collection of <see cref="ProductVariantDisplay"/></returns>
        /// <remarks>
        /// Intended to assist in product variant selection 
        /// </remarks>
        public IEnumerable<ProductVariantDisplay> GetValidProductVariants(Guid productKey, Guid[] attributeKeys)
        {
            var product = Query.Product.GetByKey(productKey);
            if (product == null) throw new InvalidOperationException("Product is null");
            if (!attributeKeys.Any()) return product.ProductVariants;

            var variants = product.ProductVariants.Where(x => attributeKeys.All(key => x.Attributes.FirstOrDefault(att => att.Key == key) != null));

            return variants;
        }

        /// <summary>
        /// Searches the Merchello Product index.  NOTE:  This returns a ProductDisplay and is not a Content search.  Use the the UmbracoHelper.Search for content searches.
        /// </summary>
        /// <param name="term">The search term</param>
        /// <returns>The collection of <see cref="ProductDisplay"/></returns>
        [Obsolete("Use MerchelloHelper.Query.Product.Search")]
        public IEnumerable<ProductDisplay> SearchProducts(string term)
        {
            return Query.Product.Search(term, 0, int.MaxValue).Items.Select(x => (ProductDisplay)x);
        }

        #endregion

        #region Invoice

        /// <summary>
        /// Retrieves a <see cref="InvoiceDisplay"/> from the Merchello Invoice index.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="InvoiceDisplay"/>.
        /// </returns>
        [Obsolete("Use MerchelloHelper.Query.Invoice.GetByKey")]
        public InvoiceDisplay Invoice(Guid key)
        {
            return Query.Invoice.GetByKey(key);
        }

        /// <summary>
        /// Retrieves a <see cref="InvoiceDisplay"/> from the Merchello Invoice index.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="InvoiceDisplay"/>.
        /// </returns>
        [Obsolete("Use MerchelloHelper.Query.Invoice.GetByKey")]
        public InvoiceDisplay Invoice(string key)
        {
            return Query.Invoice.GetByKey(key.EncodeAsGuid());
        }

        /// <summary>
        /// The invoices by customer.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// A collection of <see cref="InvoiceDisplay"/> associated with the customer.
        /// </returns>
        [Obsolete("Use MerchelloHelper.Query.Invoice.GetByCustomerKey")]
        public IEnumerable<InvoiceDisplay> InvoicesByCustomer(Guid customerKey)
        {
            return Query.Invoice.GetByCustomerKey(customerKey);
        }

        /// <summary>
        /// The invoices by customer.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// A collection of <see cref="InvoiceDisplay"/> associated with the customer.
        /// </returns>
        [Obsolete("Use MerchelloHelper.Query.Invoice.GetByCustomerKey")]
        public IEnumerable<InvoiceDisplay> InvoicesByCustomer(string customerKey)
        {
            return Query.Invoice.GetByCustomerKey(customerKey.EncodeAsGuid());
        }
        /// <summary>
        /// Searches the Merchello Invoice index. 
        /// </summary>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <returns>
        /// The collection of <see cref="InvoiceDisplay"/>.
        /// </returns>
        [Obsolete("Use MerchelloHelper.Query.Invoice.Search.  This may no longer return all valid results")]
        public IEnumerable<InvoiceDisplay> SearchInvoices(string term)
        {
            return InvoiceQuery.Search(term);
        }

        /// <summary>
        /// Searches the Merchello Invoice index. 
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// The collection of all <see cref="InvoiceDisplay"/> matching the criteria.
        /// </returns>
         [Obsolete("Use MerchelloHelper.Query.Invoice.Search.  This may no longer return all valid results")]
        public IEnumerable<InvoiceDisplay> SearchInvoices(ISearchCriteria criteria)
        {
            return InvoiceQuery.Search(criteria);
        }

        #endregion

        #region Customers

        /// <summary>
        /// The customer.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        [Obsolete("Use MerchelloHelper.Query.Customer.GetByKey")]
        public CustomerDisplay Customer(string key)
        {
            return Query.Customer.GetByKey(key.EncodeAsGuid());
        }

        /// <summary>
        /// The customer.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        [Obsolete("Use MerchelloHelper.Query.Customer.GetByKey")]
        public CustomerDisplay Customer(Guid key)
        {
            return Query.Customer.GetByKey(key);
        }

        #endregion
    }
}
