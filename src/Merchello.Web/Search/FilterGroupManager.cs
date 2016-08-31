namespace Merchello.Web.Search
{
    using System;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;
    using Merchello.Web.Services;

    /// <summary>
    /// Represents a filter group manager.
    /// </summary>
    internal class FilterGroupManager : ProxyServiceManagerBase, IFilterGroupManager
    {
        /// <summary>
        /// The <see cref="IProductFilterGroupService"/>.
        /// </summary>
        private Lazy<IProductFilterGroupService> _productFilterGroupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterGroupManager"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="proxyServiceResolver">
        /// The <see cref="IProxyEntityServiceResolver"/>.
        /// </param>
        /// <param name="collectionProviderResolver">
        /// The <see cref="IEntityCollectionProviderResolver"/>.
        /// </param>
        public FilterGroupManager(IMerchelloContext merchelloContext, IProxyEntityServiceResolver proxyServiceResolver, IEntityCollectionProviderResolver collectionProviderResolver)
            : base(merchelloContext, proxyServiceResolver)
        {
            Ensure.ParameterNotNull(collectionProviderResolver, "The IEntityCollectionProviderResolver was null");
            this.Initialize(collectionProviderResolver);
        }

        /// <summary>
        /// Gets the <see cref="IProductFilterGroupService"/>.
        /// </summary>
        public IProductFilterGroupService Product
        {
            get
            {
                return _productFilterGroupService.Value;
            }
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        /// <param name="collectionProviderResolver">
        /// The <see cref="IEntityCollectionProviderResolver"/>.
        /// </param>
        private void Initialize(IEntityCollectionProviderResolver collectionProviderResolver)
        {
            _productFilterGroupService = new Lazy<IProductFilterGroupService>(() => Resolver.Instance<ProductFilterGroupService>(new object[] { this.Services.EntityCollectionService, this.Cache, collectionProviderResolver }));
        }
    }
}