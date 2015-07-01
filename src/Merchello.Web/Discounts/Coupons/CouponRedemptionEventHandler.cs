namespace Merchello.Web.Discounts.Coupons
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Configuration;
    using Merchello.Core.Events;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The coupon redemption event handler.
    /// </summary>
    public class CouponRedemptionEventHandler : ApplicationEventHandler
    {
        /// <summary>
        /// The Umbraco Application Starting event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            SalePreparationBase.Finalizing += SalePreparationBaseOnFinalizing;
        }

        /// <summary>
        /// Records the redemption of a coupon offer.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SalePreparationBaseOnFinalizing(SalePreparationBase sender, SalesPreparationEventArgs<IPaymentResult> e)
        {
            var invoice = e.Entity.Invoice;

            // get the collection of redemptions to be record
            var visitor = new CouponRedemptionLineItemVisitor(invoice.CustomerKey);
            invoice.Items.Accept(visitor);

            if (!visitor.Redemptions.Any()) return;

            if (MerchelloContext.Current != null)
            {
                ((ServiceContext)MerchelloContext.Current.Services).OfferRedeemedService.Save(visitor.Redemptions);
            }
            else
            {
                LogHelper.Debug<CouponRedemptionEventHandler>("MerchelloContext was null.  Could not record coupon redemption.");
            }
        }
    }
}