namespace Merchello.Web.Search
{
    using System;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;

    /// <summary>
    /// Represents a filter group manager.
    /// </summary>
    internal class FilterGroupManager : ProxyQueryManagerBase, IFilterGroupManager
    {
        /// <summary>
        /// The <see cref="IProductFilterGroupQuery"/>.
        /// </summary>
        private Lazy<IProductFilterGroupQuery> _productFilterGroupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterGroupManager"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="queryManager">
        /// The <see cref="IProxyQueryManager"/>.
        /// </param>
        /// <param name="collectionProviderResolver">
        /// The <see cref="IEntityCollectionProviderResolver"/>.
        /// </param>
        public FilterGroupManager(IMerchelloContext merchelloContext, IProxyQueryManager queryManager, IEntityCollectionProviderResolver collectionProviderResolver)
            : base(merchelloContext, queryManager)
        {
            Ensure.ParameterNotNull(collectionProviderResolver, "The IEntityCollectionProviderResolver was null");
            this.Initialize(collectionProviderResolver);
        }

        /// <summary>
        /// Gets the <see cref="IProductFilterGroupQuery"/>.
        /// </summary>
        public IProductFilterGroupQuery Product
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
            _productFilterGroupService = new Lazy<IProductFilterGroupQuery>(() => this.QueryManager.Instance<ProductFilterGroupQuery>(new object[] { this.Services.EntityCollectionService, this.Cache, collectionProviderResolver }));
        }
    }
}