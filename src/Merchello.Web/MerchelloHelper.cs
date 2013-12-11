using System;
using System.Collections.Generic;
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