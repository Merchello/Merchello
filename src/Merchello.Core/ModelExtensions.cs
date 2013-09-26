using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Merchello.Core.Models;

namespace Merchello.Core
{
    public static class ModelExtensions
    {

        #region ICustomer
        
        public static void Save(ICustomer customer)
        {
            if (customer.IsDirty() && customer.HasIdentity)
            {               
                MerchelloContext.Current.Services.CustomerService.Save(customer);
            }
        }

        /// <summary>
        /// Returns a list of all customer invoices
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static IEnumerable<IInvoice> Invoices(this ICustomer customer)
        {
            if(!customer.HasIdentity) return new List<IInvoice>();
            return MerchelloContext.Current.Services.InvoiceService.GetInvoicesByCustomer(customer.Key);
        }

        #endregion


        #region IBasket

        public static void AddItem(this IProduct product)
        {
            product.ConvertToBasketItem().AddItem();
        }

        public static void AddItem(this IBasketItem basketItem)
        {
            //MerchelloContext.Current.Services.BasketService.
        }


        #endregion

        public static IBasketItem ConvertToBasketItem(this IProduct product)
        {
            return new BasketItem(1);
        }


        

        //private static T TryGetCached<T>()
        //{
        //    var cache = MerchelloContext.Current.Cache;

        //    return null;
        //}
    }
}
