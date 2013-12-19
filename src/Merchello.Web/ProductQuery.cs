using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Examine.SearchCriteria;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Examine;
using Merchello.Web.Models.ContentEditing;

namespace Merchello.Web
{
    internal class ProductQuery
    {

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> given it's 'unique' Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns><see cref="ProductDisplay"/></returns>
        public static ProductDisplay GetByKey(Guid key)
        {
            return GetByKey(key.ToString());
        }

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> given it's 'unique' Key (string representation of the Guid)
        /// </summary>
        /// <param name="key"></param>
        /// <returns><see cref="ProductDisplay"/></returns>
        public static ProductDisplay GetByKey(string key)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria(BooleanOperation.And);
            criteria.Field("productKey", key).And().Field("master", "True");
            var product = ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"]
                .Search(criteria).Select(result => result.ToProductDisplay()).FirstOrDefault();

            if (product != null) return product;
            var retrieved = MerchelloContext.Current.Services.ProductService.GetByKey(new Guid(key));
            if(retrieved != null) ReindexProduct(retrieved);

            // TODO refactor this with the ProductApiController
            AutoMapper.Mapper.CreateMap<IProduct, ProductDisplay>();
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            AutoMapper.Mapper.CreateMap<IProductOption, ProductOptionDisplay>();
            AutoMapper.Mapper.CreateMap<IProductVariant, ProductVariantDisplay>();           

            return AutoMapper.Mapper.Map<ProductDisplay>(retrieved);
        }


        /// <summary>
        /// Retrieves a <see cref="ProductVariantDisplay"/> given it's 'unique' key (string representation of the Guid)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ProductVariantDisplay GetVariantDisplayByKey(string key)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria();
            criteria.Field("productVariantKey", key);

            var variant = ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"]
                .Search(criteria).Select(result => result.ToProductVariantDisplay()).FirstOrDefault();

            if (variant != null) return variant;

            var retrieved = MerchelloContext.Current.Services.ProductVariantService.GetByKey(new Guid(key));
            if(retrieved != null) ReindexProductVariant(retrieved, null);

            return retrieved.ToProductVariantDisplay();
        }


        /// <summary>
        /// Searches ProductIndex by name and sku for the 'term' passed
        /// </summary>
        /// <param name="term"></param>
        /// <returns>A collection of <see cref="ProductDisplay"/></returns>
        public static IEnumerable<ProductDisplay> Search(string term)
        {
            var criteria = ExamineManager.Instance.CreateSearchCriteria();
            //criteria.Field("name", term.Fuzzy(0.8f)).Or().Field("sku", term);
            criteria.Field("master", "True").And().GroupedOr(new[] { "name", "sku" }, term);
            return Search(criteria);
        }

        /// <summary>
        /// Searches ProductIndex using <see cref="ISearchCriteria"/> passed
        /// </summary>
        /// <returns>A collection of <see cref="ProductDisplay"/></returns>
        public static IEnumerable<ProductDisplay> Search(ISearchCriteria criteria)
        {
            return ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"]
                .Search(criteria).OrderByDescending(x => x.Score)
                .Select(result => result.ToProductDisplay());
        }


        /// <summary>
        /// This is a sort of fall back, in the event that the index becomes corrupted or the product was not indexed for some reason.
        /// </summary>
        /// <param name="product"></param>
        private static void ReindexProduct(IProduct product)
        {
            foreach (var variant in product.ProductVariants)
            {
                ReindexProductVariant(variant, null);
            }
            ReindexProductVariant(((Product)product).MasterVariant, product.ProductOptions);

        }

        private static void ReindexProductVariant(IProductVariant productVariant, ProductOptionCollection productOptions)
        {
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"]
                .ReIndexNode(productVariant.SerializeToXml(productOptions).Root, IndexTypes.ProductVariant);
        }
     
    }
}