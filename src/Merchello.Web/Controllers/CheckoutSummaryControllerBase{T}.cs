namespace Merchello.Web.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Logging;
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
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutSummaryControllerBase{TSummary,TBillingAddress,TShippingAddress,TLineItem}"/> class. 
        /// </summary>
        protected CheckoutSummaryControllerBase()
            : this(
                  new CheckoutSummaryModelFactory<TSummary, TBillingAddress, TShippingAddress, TLineItem>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutSummaryControllerBase{TSummary,TBillingAddress,TShippingAddress,TLineItem}"/> class. 
        /// </summary>
        /// <param name="checkoutSummaryModelFactory">
        /// The <see cref="CheckoutSummaryModelFactory{TSummary, TBillingAddress, TShippingAddress, TLineItem}"/>.
        /// </param>
        protected CheckoutSummaryControllerBase(CheckoutSummaryModelFactory<TSummary, TBillingAddress, TShippingAddress, TLineItem> checkoutSummaryModelFactory)
            : this(
                  checkoutSummaryModelFactory,
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
            Ensure.ParameterNotNull(checkoutSummaryFactory, "checkoutSummaryFactory");
            this.CheckoutSummaryFactory = checkoutSummaryFactory;
        }

        #endregion

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
            if (!CheckoutManager.Payment.IsReadyToInvoice()) return InvalidCheckoutStagePartial();
            var model = CheckoutSummaryFactory.Create(CheckoutManager);
            return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
        }

        /// <summary>
        /// Renders a sales receipt.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the "invoiceKey" is not stored in the CustomerContext
        /// </exception>
        public virtual ActionResult SalesReceipt(string view = "")
        {
            var logData = MultiLogger.GetBaseLoggingData();
            logData.AddCategory("Controllers");

            var invoiceKey = CustomerContext.GetValue("invoiceKey");
            if (invoiceKey.IsNullOrWhiteSpace())
            {

                var nullRef = new NullReferenceException("The parameter invoiceKey was not found in the CustomerContext");
                MultiLogHelper.Error<CheckoutSummaryControllerBase<TSummary, TBillingAddress, TShippingAddress, TLineItem>>("The 'invoiceKey' parameter was not found in the CustomerContext", nullRef, logData);
                throw nullRef;
            }

            try
            {
                var key = new Guid(invoiceKey);
                var invoice = MerchelloServices.InvoiceService.GetByKey(key);
                var model = CheckoutSummaryFactory.Create(invoice);
                return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<CheckoutSummaryControllerBase<TSummary, TBillingAddress, TShippingAddress, TLineItem>>("Could not render the receipt.", ex, logData);
                throw;
            }
        }
    }
}