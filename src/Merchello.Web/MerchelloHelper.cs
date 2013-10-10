using System;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Umbraco.Web;

namespace Merchello.Web
{
    /// <summary>
    /// A helper class that provides many useful methods and functionality for using Merchello in templates
    /// </summary> 
    public class MerchelloHelper
    {
        //private MerchelloContext _merchelloContext;
        private UmbracoContext _umbracoContext;

        public MerchelloHelper(UmbracoContext umbracoContext)
        {

            //_merchelloContext = merchelloContext;
            _umbracoContext = umbracoContext;
        }

        #region Customer




        #endregion


    }

}