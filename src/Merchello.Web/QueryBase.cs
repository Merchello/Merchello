namespace Merchello.Web
{    
    using Core;
    using Core.Cache;
    using Core.Gateways;
    using Core.Persistence.UnitOfWork;
    using Core.Services;
    using global::Examine;
    using Umbraco.Core;

    internal abstract class QueryBase
    {
        /// <summary>
        /// Assists in unit testing
        /// </summary>
        /// <returns>
        /// The <see cref="IMerchelloContext"/>
        /// </returns>
        protected static IMerchelloContext GetMerchelloContext()
        {
            var serviceContext = new ServiceContext(new PetaPocoUnitOfWorkProvider());
            return MerchelloContext.Current ??
                new MerchelloContext(serviceContext, new GatewayContext(serviceContext, GatewayProviderResolver.Current), new CacheHelper(new NullCacheProvider(), new NullCacheProvider(), new NullCacheProvider()));
        }

        /// <summary>
        /// Rebuilds an Examine index by the index name
        /// </summary>
        /// <param name="indexName">The name (alias) of the Examine index to rebuild</param>
        protected static void RebuildIndex(string indexName)
        {
            ExamineManager.Instance.IndexProviderCollection[indexName].RebuildIndex();
        }
    }
}