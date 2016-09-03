namespace Merchello.Web.Search
{
    using Merchello.Core;
    using Merchello.Core.Services;

    using Umbraco.Core.Cache;

    /// <summary>
    /// A base class for EntityCollection proxy services.
    /// </summary>
    internal abstract class ProxyCollectionQueryBase : ProxyQueryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyCollectionQueryBase"/> class.
        /// </summary>
        /// <param name="entityCollectionService">
        /// The entity Collection Service.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        protected ProxyCollectionQueryBase(IEntityCollectionService entityCollectionService, ICacheProvider cache)
            : base(cache)
        {
            Ensure.ParameterNotNull(entityCollectionService, "The EntityCollectionService was null");
            this.Service = entityCollectionService;
   
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        protected IEntityCollectionService Service { get; private set; }
    }
}