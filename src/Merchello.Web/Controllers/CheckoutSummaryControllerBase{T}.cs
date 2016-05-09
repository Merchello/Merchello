namespace Merchello.Web.Controllers
{
    using System.Web.Mvc;

    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// A base controller for checkout summary rendering and operations.
    /// </summary>
    /// <typeparam name="TSummary">
    /// The type of the checkout summary
    /// </typeparam>
    /// <typeparam name="TBillingAddress">
    /// The type of the billing <see cref="ICheckoutAddressModel"/>
    /// </typeparam>
    /// <typeparam name="TShippingAddress">
    /// The type of the shipping <see cref="ICheckoutAddressModel"/>
    /// </typeparam>
    /// <typeparam name="TLineItem">
    /// The type of the summary <see cref="ILineItemModel"/>
    /// </typeparam>
    public abstract class CheckoutSummaryControllerBase<TSummary, TBillingAddress, TShippingAddress, TLineItem> : CheckoutControllerBase
        where TSummary : class, ICheckoutSummaryModel<TBillingAddress,TShippingAddress,TLineItem>, new() 
        where TBillingAddress : class, ICheckoutAddressModel, new()
        where TShippingAddress: class, ICheckoutAddressModel, new()
        where TLineItem: class, ILineItemModel, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutSummaryControllerBase{TSummary,TBillingAddress,TShippingAddress,TLineItem}"/> class. 
        /// </summary>
        protected CheckoutSummaryControllerBase()
            : this(new CheckoutContextSettingsFactory())
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutSummaryControllerBase{TSummary,TBillingAddress,TShippingAddress,TLineItem}"/> class. 
        /// </summary>
        /// <param name="contextSettingsFactory">
        /// The <see cref="CheckoutContextSettingsFactory"/>.
        /// </param>
        protected CheckoutSummaryControllerBase(CheckoutContextSettingsFactory contextSettingsFactory)
            : base(contextSettingsFactory)
        {
        }

        /// <summary>
        /// Renders the Basket Summary.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult BasketSummary()
        {
            var model = new TSummary();
            return this.PartialView(model);
        }
    }
}