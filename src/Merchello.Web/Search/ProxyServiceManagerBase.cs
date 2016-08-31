namespace Merchello.Web.Search
{
    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Web.Services;

    using Umbraco.Core.Cache;

    /// <summary>
    /// A base class for ProxyServiceManager classes.
    /// </summary>
    internal abstract class ProxyServiceManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyServiceManagerBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="proxyServiceResolver">
        /// The <see cref="IProxyEntityServiceResolver"/>.
        /// </param>
        protected ProxyServiceManagerBase(IMerchelloContext merchelloContext, IProxyEntityServiceResolver proxyServiceResolver)
        {
            Ensure.ParameterNotNull(merchelloContext, "MerchelloContext cannot be null");
            Ensure.ParameterNotNull(proxyServiceResolver, "The IProxyEntityServiceResolver was null.");

            this.Services = merchelloContext.Services;
            this.Resolver = proxyServiceResolver;
            this.Cache = merchelloContext.Cache.RequestCache;
        }

        /// <summary>
        /// Gets the <see cref="IServiceContext"/>.
        /// </summary>
        protected IServiceContext Services { get; private set; }

        /// <summary>
        /// Gets the <see cref="IProxyEntityServiceResolver"/>.
        /// </summary>
        protected IProxyEntityServiceResolver Resolver { get; private set; }

        /// <summary>
        /// Gets the <see cref="ICacheProvider"/>.
        /// </summary>
        protected ICacheProvider Cache { get; private set; }
    }
}