namespace Merchello.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Merchello.Core.Logging;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;

    using Umbraco.Core;

    /// <summary>
    /// A base checkout discount controller.
    /// </summary>
    /// <typeparam name="TDiscountModel">
    /// The type of <see cref="ICheckoutDiscountModel{TLineItemModel}"/>
    /// </typeparam>
    /// <typeparam name="TLineItemModel">
    /// The type of <see cref="ILineItemModel"/>
    /// </typeparam>
    /// <remarks>
    /// Applies discounts during checkout
    /// </remarks>
    public abstract class CheckoutDiscountControllerBase<TDiscountModel, TLineItemModel> : CheckoutControllerBase
        where TLineItemModel : class, ILineItemModel, new()
        where TDiscountModel : class, ICheckoutDiscountModel<TLineItemModel>, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutDiscountControllerBase{TDiscountModel,TLineItemModel}"/> class. 
        /// </summary>
        protected CheckoutDiscountControllerBase()
            : this(new CheckoutDiscountModelFactory<TDiscountModel, TLineItemModel>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutDiscountControllerBase{TDiscountModel,TLineItemModel}"/> class.
        /// </summary>
        /// <param name="checkoutDiscountModelFactory">
        /// The <see cref="CheckoutDiscountModelFactory{TDiscountModel,TLineItemModel}"/>.
        /// </param>
        protected CheckoutDiscountControllerBase(CheckoutDiscountModelFactory<TDiscountModel, TLineItemModel> checkoutDiscountModelFactory)
            : this(
                  checkoutDiscountModelFactory, 
                  new CheckoutContextSettingsFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutDiscountControllerBase{TDiscountModel,TLineItemModel}"/> class.
        /// </summary>
        /// <param name="checkoutDiscountModelFactory">
        /// The <see cref="CheckoutDiscountModelFactory{TDiscountModel,TLineItemModel}"/>.
        /// </param>
        /// <param name="contextSettingsFactory">
        /// The <see cref="CheckoutContextSettingsFactory"/>.
        /// </param>
        protected CheckoutDiscountControllerBase(
            CheckoutDiscountModelFactory<TDiscountModel, TLineItemModel> checkoutDiscountModelFactory,
            CheckoutContextSettingsFactory contextSettingsFactory)
            : base(contextSettingsFactory)
        {
            Mandate.ParameterNotNull(checkoutDiscountModelFactory, "checkoutDiscountFactory");
            this.CheckoutDiscountModelModelFactory = checkoutDiscountModelFactory;
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="CheckoutDiscountModelFactory{TDiscountModel,TLineItemModel}" />.
        /// </summary>
        protected CheckoutDiscountModelFactory<TDiscountModel, TLineItemModel> CheckoutDiscountModelModelFactory { get; private set; }

        /// <summary>
        /// Attempts to apply a discount to the sale / order.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Throws an invalid operation exception if the CheckoutManager is not ready to invoice.
        /// </exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ApplyDiscount(TDiscountModel model)
        {
            if (!CheckoutManager.Payment.IsReadyToInvoice())
            {
                var logData = MultiLogger.GetBaseLoggingData();
                var invalidOp = new InvalidOperationException("Cannot apply discount when CheckoutManager is not ready to invoice");
                MultiLogHelper.Error<CheckoutDiscountControllerBase<TDiscountModel, TLineItemModel>>("Cannot apply discount", invalidOp, logData);
                throw invalidOp;
            }

            var redeemResult = CheckoutManager.Offer.RedeemCouponOffer(model.OfferCode);
            var resultModel = CheckoutDiscountModelModelFactory.Create(redeemResult);

            resultModel.OfferCode = model.OfferCode;

            return HandleApplyDiscountSuccess(resultModel);
        }

        /// <summary>
        /// Renders the discount form.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult DiscountForm(string view = "")
        {
            // Discounts must be applied to an invoice (not a basket) due to the various validation options/constraints
            // that can be applied to a discount (some of which cannot be determined with a simple basket).
            if (!CheckoutManager.Payment.IsReadyToInvoice()) return PartialView("InvalidCheckoutStage");

            var model = CheckoutDiscountModelModelFactory.Create();

            return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
        }

        /// <summary>
        /// Handles a success apply discount operation.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult HandleApplyDiscountSuccess(TDiscountModel model)
        {
            ViewData["MerchelloViewData"] = model.ViewData;
            return CurrentUmbracoPage();
        }
    }
}