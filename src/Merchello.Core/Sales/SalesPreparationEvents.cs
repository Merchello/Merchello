namespace Merchello.Core.Sales
{
    using Events;
    using Gateways.Payment;
    using Models;
    using Umbraco.Core;

    /// <summary>
    /// Handles sales preparation events.
    /// </summary>
    public class SalesPreparationEvents : ApplicationEventHandler
    {
        /// <summary>
        /// The Umbraco application started event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The Umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The Umbraco application context.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            SalePreparationBase.Finalizing += SalePreparationBaseOnFinalizing;
        }

        /// <summary>
        /// Handles the <see cref="SalePreparationBase"/> finalizing event
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The <see cref="IPaymentResult"/>
        /// </param>
        private void SalePreparationBaseOnFinalizing(SalePreparationBase sender, SalesPreparationEventArgs<IPaymentResult> args)
        {
            var result = args.Entity;

            if (!result.ApproveOrderCreation) return;

            // order
            var order = result.Invoice.PrepareOrder(MerchelloContext.Current);

            MerchelloContext.Current.Services.OrderService.Save(order);
        }
    }
}