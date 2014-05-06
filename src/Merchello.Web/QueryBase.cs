using Examine;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
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
            var serviceContext = new ServiceContext(new PetaPocoUnitOfWorkProvider());
            return MerchelloContext.Current ??
                new MerchelloContext(serviceContext,
                    new GatewayContext(serviceContext, GatewayProviderResolver.Current),
                        new CacheHelper(new NullCacheProvider(), new NullCacheProvider(), new NullCacheProvider()));

        }

        protected static void RebuildIndex(string indexName)
        {
            ExamineManager.Instance.IndexProviderCollection[indexName].RebuildIndex();
        }
    }
}