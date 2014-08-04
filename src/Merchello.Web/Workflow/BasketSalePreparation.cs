namespace Merchello.Web.Workflow
{
    using System.Linq;
    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;

    /// <summary>
    /// Represents the basket sale preparation.
    /// </summary>
    public class BasketSalePreparation : SalePreparationBase, IBasketSalePreparation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasketSalePreparation"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        internal BasketSalePreparation(IMerchelloContext merchelloContext, IItemCache itemCache, ICustomerBase customer)
            : base(merchelloContext, itemCache, customer)
        {
        }

        /// <summary>
        /// Attempts to authorize a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public override IPaymentResult AuthorizePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            var result = base.AuthorizePayment(paymentGatewayMethod, args);

            if (result.Payment.Success) Customer.Basket().Empty();
            
            return result;
        }

        /// <summary>
        /// Authorizes and Captures a Payment 
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public override IPaymentResult AuthorizeCapturePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            var result = base.AuthorizeCapturePayment(paymentGatewayMethod, args);

            if (result.Payment.Success) Customer.Basket().Empty();

            return result;
        }

        /// <summary>
        /// The get basket checkout preparation.
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <returns>
        /// The <see cref="BasketSalePreparation"/>.
        /// </returns>
        internal static BasketSalePreparation GetBasketCheckoutPreparation(IBasket basket)
        {
            return GetBasketCheckoutPreparation(Core.MerchelloContext.Current, basket);
        }

        /// <summary>
        /// The get basket checkout preparation.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <returns>
        /// The <see cref="BasketSalePreparation"/>.
        /// </returns>
        internal static BasketSalePreparation GetBasketCheckoutPreparation(IMerchelloContext merchelloContext, IBasket basket)
        {
            var customer = basket.Customer;
            var itemCache = GetItemCache(merchelloContext, customer, basket.VersionKey);

            if (!itemCache.Items.Any())
            {
                // this is either a new preparation or a reset due to version
                foreach (var item in basket.Items)
                {
                    // convert to a LineItem of the same type for use in the CheckoutPrepartion collection
                    itemCache.AddItem(item.AsLineItemOf<ItemCacheLineItem>());
                }
            }

            return new BasketSalePreparation(merchelloContext, itemCache, customer);
        }
    }
}