using Examine;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Umbraco.Core;

namespace Merchello.Web
{
    internal  abstract class QueryBase
    {
        /// <summary>
        /// Assists in unit testing
        /// </summary>
        /// <returns></returns>
        protected static IMerchelloContext GetMerchelloContext()
        {
            return MerchelloContext.Current ??
                new MerchelloContext(new ServiceContext(new PetaPocoUnitOfWorkProvider()),
                                        new CacheHelper(new NullCacheProvider(), new NullCacheProvider(), new NullCacheProvider()));

        }

        protected static void RebuildIndex(string indexName)
        {
            ExamineManager.Instance.IndexProviderCollection[indexName].RebuildIndex();
        }
    }
}