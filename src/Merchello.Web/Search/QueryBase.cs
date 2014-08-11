namespace Merchello.Web
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Core;
    using Core.Cache;
    using Core.Gateways;
    using Core.Persistence.UnitOfWork;
    using Core.Services;
    using global::Examine;
    using global::Examine.LuceneEngine.SearchCriteria;
    using global::Examine.Providers;
    using global::Examine.SearchCriteria;

    using Merchello.Examine;
    using Merchello.Web.Search;

    using Umbraco.Core;

    /// <summary>
    /// The query base.
    /// </summary>
    public abstract class QueryBase
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

        /// <summary>
        /// Builds search criteria
        /// </summary>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="fields">
        /// The fields.
        /// </param>
        /// <returns>
        /// The <see cref="ISearchCriteria"/>.
        /// </returns>
        protected static ISearchCriteria BuildCriteria(string providerName, string searchTerm, string[] fields)
        {
            return SearchHelper.BuildCriteria(searchTerm, providerName, fields);
        }
    }
}