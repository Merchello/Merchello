namespace Merchello.Web.Workflow
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;

    /// <summary>
    /// Represents the backoffice sale preparation.
    /// </summary>
    public class BackofficeSalePreparation : SalePreparationBase, IBackofficeSalePreparation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackofficeSalePreparation"/> class.
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
        internal BackofficeSalePreparation(IMerchelloContext merchelloContext, IItemCache itemCache, ICustomerBase customer)
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

            Customer.Backoffice().Empty();

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

            Customer.Backoffice().Empty();

            return result;
        }

        /// <summary>
        /// The get backoffice checkout preparation.
        /// </summary>
        /// <param name="backoffice">
        /// The backoffice.
        /// </param>
        /// <returns>
        /// The <see cref="BackofficeSalePreparation"/>.
        /// </returns>
        internal static BackofficeSalePreparation GetBackofficeCheckoutPreparation(IBackoffice backoffice)
        {
            return GetBackofficeCheckoutPreparation(Core.MerchelloContext.Current, backoffice);
        }

        /// <summary>
        /// The get backoffice checkout preparation.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="backoffice">
        /// The backoffice.
        /// </param>
        /// <returns>
        /// The <see cref="BackofficeSalePreparation"/>.
        /// </returns>
        internal static BackofficeSalePreparation GetBackofficeCheckoutPreparation(IMerchelloContext merchelloContext, IBackoffice backoffice)
        {
            var customer = backoffice.Customer;
            var itemCache = GetItemCache(merchelloContext, customer, backoffice.VersionKey);

            if (!itemCache.Items.Any())
            {
                // this is either a new preparation or a reset due to version
                foreach (var item in backoffice.Items)
                {
                    // convert to a LineItem of the same type for use in the CheckoutPrepartion collection
                    itemCache.AddItem(item.AsLineItemOf<ItemCacheLineItem>());
                }
            }

            return new BackofficeSalePreparation(merchelloContext, itemCache, customer);
        }
    }
}