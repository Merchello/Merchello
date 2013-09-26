using System;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Web
{
    /// <summary>
    /// A helper class that provides many useful methods and functionality for using Merchello in templates
    /// </summary> 
    public class MerchelloHelper
    {
        
        #region Customer

        /// <summary>
        /// Gets a <see cref="ICustomer"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ICustomer Customer(string key)
        {
            return Customer(new Guid(key));
        }

        /// <summary>
        /// Gets a <see cref="ICustomer"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ICustomer Customer(Guid key)
        {
            return MerchelloContext.Current.Services.CustomerService.GetByKey(key);
        }

        #endregion

        #region ProductVariant

        public IProductVariant ProductVariant(string key)
        {
            
        }

        public IProductVariant ProductVariant(Guid key)
        {
            return new ProductVariant();
        }



        #endregion
    }

    public class ProductVariant : IProductVariant
    {
    }

    public interface IProductVariant
    {
    }
}