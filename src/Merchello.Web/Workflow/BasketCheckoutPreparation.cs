using Merchello.Core;
using Merchello.Core.Checkout;
using Merchello.Core.Models;

namespace Merchello.Web.Workflow
{
    public class BasketCheckoutPreparation : CheckoutPreparationBase, IBasketCheckoutPreparation 
    {
        internal BasketCheckoutPreparation(IMerchelloContext merchelloContext, IItemCache itemCache, ICustomerBase customer) 
            : base(merchelloContext, itemCache, customer)
        { }

        internal static BasketCheckoutPreparation GetBasketCheckoutPreparation(IBasket basket)
        {
            return GetBasketCheckoutPreparation(Core.MerchelloContext.Current, basket);
        }

        internal static BasketCheckoutPreparation GetBasketCheckoutPreparation(IMerchelloContext merchelloContext, IBasket basket)
        {
            var customer = basket.Customer;
            var itemCache = GetItemCache(merchelloContext, customer, basket.VersionKey);
            foreach (var item in basket.Items)
            {
                // convert to a LineItem of the same type for use in the CheckoutPrepartion collection
                itemCache.AddItem(item.AsLineItemOf<ItemCacheLineItem>());
            }
            return new BasketCheckoutPreparation(merchelloContext, itemCache, customer);
        }

    }
}