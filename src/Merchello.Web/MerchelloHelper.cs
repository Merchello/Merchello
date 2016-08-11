namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core;
    using Core.Services;
    
    using global::Examine.SearchCriteria;

    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.ValueConverters;
    using Merchello.Web.Caching;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Validation;

    using Models.ContentEditing;
    using Search;
    using Umbraco.Core;

    /// <summary>
    /// A helper class that provides many useful methods and functionality for using Merchello in templates
    /// </summary>
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
        /// The <see cref="ProductContentFactory"/>.
        /// </summary>
        private readonly Lazy<ProductContentFactory> _productContentFactory;

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
            : this(MerchelloContext.Current, enableDataModifiers)
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
        [Obsolete("Use either the default constructor or the constructor that takes the MerchelloContext as an argument")]
        public MerchelloHelper(IServiceContext serviceContext, bool enableDataModifiers)
            : this(MerchelloContext.Current, enableDataModifiers, DetachedValuesConversionType.Db)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloHelper"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not to enable data modifiers
        /// </param>
        public MerchelloHelper(IMerchelloContext merchelloContext, bool enableDataModifiers)
            : this(merchelloContext, enableDataModifiers, DetachedValuesConversionType.Db)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloHelper"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="enableDataModifiers">
        /// The enable data modifiers.
        /// </param>
        /// <param name="conversionType">
        /// The conversion type for detached values.
        /// </param>
        internal MerchelloHelper(IMerchelloContext merchelloContext, bool enableDataModifiers, DetachedValuesConversionType conversionType)
        {
            Mandate.ParameterNotNull(merchelloContext, "ServiceContext cannot be null");

            _enableDataModifiers = enableDataModifiers;
            _queryProvider = new Lazy<ICachedQueryProvider>(() => new CachedQueryProvider(merchelloContext, _enableDataModifiers, conversionType));
            _validationHelper = new Lazy<IValidationHelper>(() => new ValidationHelper());
            _productContentFactory = new Lazy<ProductContentFactory>(() => new ProductContentFactory());
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
            return Query.Product.TypedProductContent(key);
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
            return Query.Product.TypedProductContentBySlug(slug);
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
            return Query.Product.TypedProductContentBySku(sku);
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
            return Query.Product.TypedProductContentFromCollection(collectionKey);
        }

        /// <summary>
        /// The typed product content from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="page">
        /// The current page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items Per Page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field (valid values are "sku", "name", "price").
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public IEnumerable<IProductContent> TypedProductContentFromCollection(Guid collectionKey, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending)
        {
            if (page <= 0) page = 1;

            return Query.Product.TypedProductContentFromCollection(collectionKey, page, itemsPerPage, sortBy, sortDirection);
        }

        /// <summary>
        /// The type product content.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public IEnumerable<IProductContent> TypeProductContent(IEnumerable<Guid> keys) // productKeys not productVariantKeys
        {
            return keys.Select(TypedProductContent);
        }

        /// <summary>
        /// Formats an amount based on Merchello store settings.
        /// </summary>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <returns>
        /// The formatted currency.
        /// </returns>
        public string FormatCurrency(decimal amount)
        {
            return CurrencyHelper.FormatCurrency(amount);
        }

    }
}
