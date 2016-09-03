namespace Merchello.Web.Search
{
    using Merchello.Core;
    using Merchello.Core.Services;

    using Umbraco.Core.Cache;

    /// <summary>
    /// A base class for ProxyServiceManager classes.
    /// </summary>
    internal abstract class ProxyQueryManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyQueryManagerBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="queryManager">
        /// The <see cref="IProxyQueryManager"/>.
        /// </param>
        protected ProxyQueryManagerBase(IMerchelloContext merchelloContext, IProxyQueryManager queryManager)
        {
            Ensure.ParameterNotNull(merchelloContext, "MerchelloContext cannot be null");
            Ensure.ParameterNotNull(queryManager, "The IProxyEntityServiceResolver was null.");

            this.Services = merchelloContext.Services;
            this.QueryManager = queryManager;
            this.Cache = merchelloContext.Cache.RequestCache;
        }

        /// <summary>
        /// Gets the <see cref="IServiceContext"/>.
        /// </summary>
        protected IServiceContext Services { get; private set; }

        /// <summary>
        /// Gets the <see cref="IProxyQueryManager"/>.
        /// </summary>
        protected IProxyQueryManager QueryManager { get; private set; }

        /// <summary>
        /// Gets the <see cref="ICacheProvider"/>.
        /// </summary>
        protected ICacheProvider Cache { get; private set; }
    }
}