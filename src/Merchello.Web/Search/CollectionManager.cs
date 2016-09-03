namespace Merchello.Web.Search
{
    using System;

    using Merchello.Core;

    /// <summary>
    /// Represents a collection manager.
    /// </summary>
    internal class CollectionManager : ProxyQueryManagerBase, ICollectionManager
    {

        /// <summary>
        /// Lazy for access to the <see cref="IProductCollectionQuery"/>.
        /// </summary>
        private Lazy<IProductCollectionQuery> _productCollectionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionManager"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>
        /// </param>
        /// <param name="queryManager">
        /// The resolver.
        /// </param>
        public CollectionManager(IMerchelloContext merchelloContext, IProxyQueryManager queryManager)
            : base(merchelloContext, queryManager)
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets the <see cref="IProductCollectionQuery"/>.
        /// </summary>
        public IProductCollectionQuery Product
        {
            get
            {
                return this._productCollectionService.Value;
            }
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        private void Initialize()
        {
            this._productCollectionService = new Lazy<IProductCollectionQuery>(() => this.QueryManager.Instance<ProductCollectionQuery>(new object[] { this.Services.EntityCollectionService, this.Cache }));
        }
    }
}