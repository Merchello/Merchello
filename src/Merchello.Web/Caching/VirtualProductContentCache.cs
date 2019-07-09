namespace Merchello.Web.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Models;

    /// <summary>
    /// A cache for <see cref="IProductContent"/>.
    /// </summary>
    internal sealed class VirtualProductContentCache : VirtualContentCache<IProductContent, IProduct>, IVirtualProductContentCache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualProductContentCache"/> class.
        /// </summary>
        public VirtualProductContentCache()
            : this(ApplicationContext.Current.ApplicationCache)
        {
        }

        /// <summary>
        /// Changes the product display content by ui culture.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private IProductContent UpdateLanguage(IProductContent content)
        {
            content?.SpecifyCulture(System.Threading.Thread.CurrentThread.CurrentUICulture);
            return content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualProductContentCache"/> class.
        /// </summary>
        /// <param name="cache">
        /// The cache.
        /// </param>
        public VirtualProductContentCache(CacheHelper cache)
            : this(cache, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualProductContentCache"/> class.
        /// </summary>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="modified">
        /// The modified.
        /// </param>
        public VirtualProductContentCache(CacheHelper cache, bool modified)
            : this(cache, null, modified)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualProductContentCache"/> class.
        /// </summary>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="fetch">
        /// The fetch.
        /// </param>
        /// <param name="modified">
        /// The modified.
        /// </param>
        public VirtualProductContentCache(CacheHelper cache, Func<Guid, IProductContent> fetch, bool modified)
            : base(cache, fetch, modified)
        {
        }

        /// <summary>
        /// Gets <see cref="IPublishedContent"/> by it's slug.
        /// </summary>
        /// <param name="slug">
        /// The slug.
        /// </param>
        /// <param name="get">
        /// A function to get by slug as a fallback
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public IProductContent GetBySlug(string slug, Func<string, IProductContent> get)
        {
            var cacheKey = GetSlugCacheKey(slug, ModifiedVersion);
            var content = (IProductContent)Cache.RuntimeCache.GetCacheItem(cacheKey);
            if (content != null) return UpdateLanguage(content);

            return UpdateLanguage(CacheContent(cacheKey, get.Invoke(slug)));
        }

        /// <summary>
        /// Gets <see cref="IPublishedContent"/> by it's sku.
        /// </summary>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <param name="get">
        /// A function to get by sku as a fallback
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public IProductContent GetBySku(string sku, Func<string, IProductContent> get)
        {
             var cacheKey = GetSkuCacheKey(sku, ModifiedVersion);
            var content = (IProductContent)Cache.RuntimeCache.GetCacheItem(cacheKey);
            if (content != null) return UpdateLanguage(content);

            return UpdateLanguage(CacheContent(cacheKey, get.Invoke(sku)));
        }

        /// <summary>
        /// Clears the virtual content cache.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public override void ClearVirtualCache(DeleteEventArgs<IProduct> e)
        {
            base.ClearVirtualCache(e);

            RemoveFromCache(e.DeletedEntities);
        }

        /// <summary>
        /// Clears the virtual content cache.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public override void ClearVirtualCache(SaveEventArgs<IProduct> e)
        {
            base.ClearVirtualCache(e);

            RemoveFromCache(e.SavedEntities);
        }

        /// <summary>
        /// Clears the virtual content cache.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        public void ClearVirtualCache(IProduct product)
        {
            ClearVirtualCache(product.Key);

            RemoveFromCache(product);
        }

        /// <summary>
        /// Gets the slug cache key.
        /// </summary>
        /// <param name="slug">
        /// The slug.
        /// </param>
        /// <param name="modified">
        /// The modified.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetSlugCacheKey(string slug, bool modified)
        {
            return string.Format("merch.productcontent.slug.{0}.{1}", slug, modified);
        }

        /// <summary>
        /// Gets the sku cache key.
        /// </summary>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <param name="modified">
        /// The modified.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetSkuCacheKey(string sku, bool modified)
        {
            return string.Format("merch.productcontent.sku.{0}.{1}", sku, modified);
        }

        /// <summary>
        /// Clears the virtual content cache.
        /// </summary>
        /// <param name="products">
        /// The products.
        /// </param>
        private void RemoveFromCache(IEnumerable<IProduct> products)
        {
            foreach (var p in products)
            {
                RemoveFromCache(p);
            }
        }

        /// <summary>
        /// Clears the virtual content cache.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        private void RemoveFromCache(IProduct product)
        {
            if (product.DetachedContents.Any())
            {
                foreach (var dc in product.DetachedContents.Where(x => !x.Slug.IsNullOrWhiteSpace()))
                {
                    Cache.RuntimeCache.ClearCacheItem(GetSlugCacheKey(dc.Slug, true));
                    Cache.RuntimeCache.ClearCacheItem(GetSlugCacheKey(dc.Slug, false));
                }
            }

            Cache.RuntimeCache.ClearCacheItem(GetSkuCacheKey(product.Sku, true));
            Cache.RuntimeCache.ClearCacheItem(GetSkuCacheKey(product.Sku, false));
        }
    }
}
