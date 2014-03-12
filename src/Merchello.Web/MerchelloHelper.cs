using System;
using System.Collections.Generic;
using System.Linq;
using Examine.SearchCriteria;
using Merchello.Web.Models.ContentEditing;

namespace Merchello.Web
{
    /// <summary>
    /// A helper class that provides many useful methods and functionality for using Merchello in templates
    /// </summary> 
    public class MerchelloHelper
    {
        
        #region Product

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> from the Merchello Product index.
        /// </summary>
        public ProductDisplay Product(string key)
        {
            return ProductQuery.GetByKey(key);
        }

        /// <summary>
        /// Retrieves a <see cref="ProductDisplay"/> from the Merchello Product index.
        /// </summary>
        public ProductDisplay Product(Guid key)
        {
            return ProductQuery.GetByKey(key);
        }

        /// <summary>
        /// Returns a collection of all <see cref="ProductDisplay"/>
        /// </summary>
        public IEnumerable<ProductDisplay> AllProducts()
        {
            return ProductQuery.GetAllProducts();
        }

        /// <summary>
        /// Retrieves a <see cref="ProductVariantDisplay"/> from the Merchello Product index.
        /// </summary>
        public ProductVariantDisplay ProductVariant(string key)
        {
            return ProductQuery.GetVariantDisplayByKey(key);
        }

        /// <summary>
        /// Retrieves a <see cref="ProductVariantDisplay"/> from the Merchello Product index.
        /// </summary>
        public ProductVariantDisplay ProductVariant(Guid key)
        {
            return ProductVariant(key.ToString());
        }


        /// <summary>
        /// Get a product variant from a product by it's collection of attributes
        /// </summary>
        /// <param name="productKey">The product key</param>
        /// <param name="attributeKeys">The option choices (attributeKeys)</param>
        /// <returns></returns>
        public ProductVariantDisplay GetProductVariantWithAttributes(Guid productKey, Guid[] attributeKeys)
        {
            var product = Product(productKey);
            return product.ProductVariants.FirstOrDefault(x => x.Attributes.Count() == attributeKeys.Count() && attributeKeys.All(key => x.Attributes.FirstOrDefault(att => att.Key == key) != null));
        }

        /// <summary>
        /// Gets a list of valid variants based on partial attribute selection
        /// </summary>
        /// <param name="productKey">The product key</param>
        /// <param name="attributeKeys">The selected option choices</param>
        /// <returns>A collection of <see cref="ProductVariantDisplay"/></returns>
        /// <remarks>
        /// Intended to assist in product variant selection 
        /// </remarks>
        public IEnumerable<ProductVariantDisplay> GetValidProductVaraints(Guid productKey, Guid[] attributeKeys)
        {
            var product = Product(productKey);
            if(product == null) throw new InvalidOperationException("Product is null");
            if (!attributeKeys.Any()) return product.ProductVariants;

            return product.ProductVariants.Where(x => attributeKeys.All(key => x.Attributes.FirstOrDefault(att => att.Key == key) != null));
        }

        /// <summary>
        /// Searches the Merchello Product index.  NOTE:  This returns a ProductDisplay and is not a Content search.  Use the the UmbracoHelper.Search for content searches.
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public IEnumerable<ProductDisplay> SearchProducts(string term)
        {
            return ProductQuery.Search(term);
        }

        /// <summary>
        /// Searches the Merchello Product index.  NOTE:  This returns a ProductDisplay and is not a Content search.  Use the the UmbracoHelper.Search for content searches.
        /// </summary>
        public IEnumerable<ProductDisplay> SearchProducts(ISearchCriteria criteria)
        {
            return ProductQuery.Search(criteria);
        }

        #endregion


    }

}