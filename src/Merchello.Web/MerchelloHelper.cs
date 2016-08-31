namespace Merchello.Web
{
    using System;

    using Core;
    using Core.Services;

    using Merchello.Core.EntityCollections;
    using Merchello.Core.ValueConverters;
    using Merchello.Web.Services;
    using Merchello.Web.Validation;

    using Search;
    using Umbraco.Core;

    /// <summary>
    /// A helper class that provides many useful methods and functionality for using Merchello in templates
    /// </summary>
    //// REFACTOR this class is not technically a "helper" class since it has to be instantiated. 
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
            : this(merchelloContext, enableDataModifiers, conversionType, ProxyEntityServiceResolver.Current, EntityCollectionProviderResolver.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloHelper"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="enableDataModifiers">
        /// The enable data modifiers.
        /// </param>
        /// <param name="conversionType">
        /// The conversion type.
        /// </param>
        /// <param name="proxyServiceResolver">
        /// The proxy service resolver.
        /// </param>
        /// <param name="collectionProviderResolver">
        /// The <see cref="IEntityCollectionProviderResolver"/>
        /// </param>
        internal MerchelloHelper(
            IMerchelloContext merchelloContext,
            bool enableDataModifiers,
            DetachedValuesConversionType conversionType,
            IProxyEntityServiceResolver proxyServiceResolver,
            IEntityCollectionProviderResolver collectionProviderResolver)
        {
            Ensure.ParameterNotNull(merchelloContext, "ServiceContext cannot be null");
            Ensure.ParameterNotNull(proxyServiceResolver, "The IProxyEntityServicerResolver was null");
            Ensure.ParameterNotNull(collectionProviderResolver, "The IEntityCollectionProviderResolver was null");
            _enableDataModifiers = enableDataModifiers;
            _queryProvider = new Lazy<ICachedQueryProvider>(() => new CachedQueryProvider(merchelloContext, _enableDataModifiers, conversionType));
            _validationHelper = new Lazy<IValidationHelper>(() => new ValidationHelper());

            this.Initialize(merchelloContext, proxyServiceResolver, collectionProviderResolver);
        }

        /// <summary>
        /// Gets the <see cref="ICollectionManager"/>.
        /// </summary>
        public ICollectionManager Collections { get; private set; }

        /// <summary>
        /// Gets the <see cref="IFilterGroupManager"/>.
        /// </summary>
        public IFilterGroupManager Filters { get; private set; }

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

        /// <summary>
        /// Initializes the MerchelloHelper.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="proxyServiceResolver">
        /// The resolver.
        /// </param>
        /// <param name="collectionProviderResolver">
        /// The collection Provider Resolver.
        /// </param>
        private void Initialize(IMerchelloContext merchelloContext, IProxyEntityServiceResolver proxyServiceResolver, IEntityCollectionProviderResolver collectionProviderResolver)
        {
            this.Collections = new CollectionManager(merchelloContext, proxyServiceResolver);

            this.Filters = new FilterGroupManager(merchelloContext, proxyServiceResolver, collectionProviderResolver);
        }
    }
}
