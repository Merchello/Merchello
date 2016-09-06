namespace Merchello.Umbraco.Adapters
{
    using Merchello.Core.Cache;

    /// <summary>
    /// The cache helper adapter.
    /// </summary>
    internal class CacheHelperAdapter : CacheHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHelperAdapter"/> class. 
        /// <para>An adapter to use Umbraco's <see>
        ///         <cref>global::Umbraco.Core.CacheHelper</cref>
        ///     </see>
        ///     as <see cref="CacheHelper"/></para>
        /// </summary>
        /// <param name="umbCacheHelper">
        /// The umb Cache Helper.
        /// </param>
        /// TODO CacheHelper has moved to Cache namespace in V8
        public CacheHelperAdapter(global::Umbraco.Core.CacheHelper umbCacheHelper)
            : base(
                  new RuntimeCacheProviderAdapter(umbCacheHelper.RuntimeCache),
                  new CacheProviderAdapter(umbCacheHelper.StaticCache),
                  new CacheProviderAdapter(umbCacheHelper.RequestCache))
        {
        }
    }
}