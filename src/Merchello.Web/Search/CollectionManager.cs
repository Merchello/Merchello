namespace Merchello.Web.Search
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Web.Services;

    using Umbraco.Core.Cache;

    /// <summary>
    /// Represents a collection manager.
    /// </summary>
    internal class CollectionManager : ProxyServiceManagerBase, ICollectionManager
    {

        /// <summary>
        /// Lazy for access to the <see cref="IProductCollectionService"/>.
        /// </summary>
        private Lazy<IProductCollectionService> _productCollectionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionManager"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>
        /// </param>
        /// <param name="proxyServiceResolver">
        /// The resolver.
        /// </param>
        public CollectionManager(IMerchelloContext merchelloContext, IProxyEntityServiceResolver proxyServiceResolver)
            : base(merchelloContext, proxyServiceResolver)
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets the <see cref="IProductCollectionService"/>.
        /// </summary>
        public IProductCollectionService Product
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
            this._productCollectionService = new Lazy<IProductCollectionService>(() => this.Resolver.Instance<ProductCollectionService>(new object[] { this.Services.EntityCollectionService, this.Cache }));
        }
    }
}