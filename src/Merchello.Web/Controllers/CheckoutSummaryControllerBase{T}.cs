namespace Merchello.Web.Controllers
{
    using System.Web.Mvc;

    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;

    using Umbraco.Core;

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
        where TShippingAddress : class, ICheckoutAddressModel, new()
        where TLineItem : class, ILineItemModel, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutSummaryControllerBase{TSummary,TBillingAddress,TShippingAddress,TLineItem}"/> class. 
        /// </summary>
        protected CheckoutSummaryControllerBase()
            : this(
                  new CheckoutSummaryModelFactory<TSummary, TBillingAddress, TShippingAddress, TLineItem>(),
                  new CheckoutContextSettingsFactory())
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutSummaryControllerBase{TSummary,TBillingAddress,TShippingAddress,TLineItem}"/> class. 
        /// </summary>
        /// <param name="checkoutSummaryFactory">
        /// The <see cref="CheckoutSummaryControllerBase{TSummary, TBillingAddress, TShippingAddress, TLineItem}"/>.
        /// </param>
        /// <param name="contextSettingsFactory">
        /// The <see cref="CheckoutContextSettingsFactory"/>.
        /// </param>
        protected CheckoutSummaryControllerBase(
            CheckoutSummaryModelFactory<TSummary, TBillingAddress, TShippingAddress, TLineItem> checkoutSummaryFactory,
            CheckoutContextSettingsFactory contextSettingsFactory)
            : base(contextSettingsFactory)
        {
            Mandate.ParameterNotNull(checkoutSummaryFactory, "checkoutSummaryFactory");
            this.CheckoutSummaryFactory = checkoutSummaryFactory;
        }

        /// <summary>
        /// Gets the <see cref="CheckoutSummaryModelFactory{TSummary, TBillingAddress, TShippingAddress, TLineItem}"/>.
        /// </summary>
        protected CheckoutSummaryModelFactory<TSummary, TBillingAddress, TShippingAddress, TLineItem> CheckoutSummaryFactory { get; private set; }

        /// <summary>
        /// Renders the Basket Summary.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult BasketSummary(string view = "")
        {
            var model = CheckoutSummaryFactory.Create(Basket, CheckoutManager);
            return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
        }

        /// <summary>
        /// Renders the Invoice summary.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult InvoiceSummary(string view = "")
        {
            var model = CheckoutSummaryFactory.Create(CheckoutManager);
            return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
        }
    }
}