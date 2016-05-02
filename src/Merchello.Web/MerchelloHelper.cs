namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core;
    using Core.Services;
    
    using global::Examine.SearchCriteria;

    using Merchello.Core.ValueConverters;
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
            : this(serviceContext, enableDataModifiers, DetachedValuesConversionType.Db)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloHelper"/> class.
        /// </summary>
        /// <param name="serviceContext">
        /// The service context.
        /// </param>
        /// <param name="enableDataModifiers">
        /// The enable data modifiers.
        /// </param>
        /// <param name="conversionType">
        /// The conversion type for detached values.
        /// </param>
        internal MerchelloHelper(IServiceContext serviceContext, bool enableDataModifiers, DetachedValuesConversionType conversionType)
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
