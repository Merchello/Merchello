using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Examine.SearchCriteria;
using Merchello.Core.Models;
using Merchello.Web.Cache;
using Merchello.Web.Models.ContentEditing;
using Umbraco.Web;

namespace Merchello.Web
{
    /// <summary>
    /// A helper class that provides many useful methods and functionality for using Merchello in templates
    /// </summary> 
    public class MerchelloHelper
    {
        
        #region Product

        public ProductDisplay Product(string key)
        {
            return ProductQuery.GetByKey(key);
        }

        public ProductDisplay Product(Guid key)
        {
            return ProductQuery.GetByKey(key);
        }

        public IEnumerable<ProductDisplay> SearchProducts(string term)
        {
            return ProductQuery.Search(term);
        }

        public IEnumerable<ProductDisplay> SearchProducts(ISearchCriteria criteria)
        {
            return ProductQuery.Search(criteria);
        }

        #endregion


    }

}