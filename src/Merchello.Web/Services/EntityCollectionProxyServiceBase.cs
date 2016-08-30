namespace Merchello.Web.Services
{
    using Merchello.Core;
    using Merchello.Core.Services;

    using Umbraco.Core.Cache;

    /// <summary>
    /// A base class for EntityCollection proxy services.
    /// </summary>
    internal abstract class EntityCollectionProxyServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionProxyServiceBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        protected EntityCollectionProxyServiceBase(IMerchelloContext merchelloContext)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");
            this.Service = merchelloContext.Services.EntityCollectionService;
            this.Cache = merchelloContext.Cache.RequestCache;
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        protected ICacheProvider Cache { get; private set; }

        /// <summary>
        /// Gets the service.
        /// </summary>
        protected IEntityCollectionService Service { get; private set; }
    }
}