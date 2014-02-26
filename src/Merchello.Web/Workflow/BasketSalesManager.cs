using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Sales;

namespace Merchello.Web.Workflow
{
    /// <summary>
    /// Represents a BaskeOrderPreparation
    /// </summary>
    public class BasketSalesManager : SalesManagerBase, IBasketSalesManager
    {
        internal BasketSalesManager(IMerchelloContext merchelloContext, IItemCache itemCache, ICustomerBase customer) 
            : base(merchelloContext, itemCache, customer)
        { }

        internal static BasketSalesManager GetBasketCheckoutPreparation(IBasket basket)
        {
            return GetBasketCheckoutPreparation(Core.MerchelloContext.Current, basket);
        }

        internal static BasketSalesManager GetBasketCheckoutPreparation(IMerchelloContext merchelloContext, IBasket basket)
        {
            var customer = basket.Customer;
            var itemCache = GetItemCache(merchelloContext, customer, basket.VersionKey);
            foreach (var item in basket.Items)
            {
                // convert to a LineItem of the same type for use in the CheckoutPrepartion collection
                itemCache.AddItem(item.AsLineItemOf<ItemCacheLineItem>());
            }
            return new BasketSalesManager(merchelloContext, itemCache, customer);
        }

    }
}