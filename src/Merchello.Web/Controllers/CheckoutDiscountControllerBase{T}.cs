namespace Merchello.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Web.Discounts.Coupons;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.ContentEditing;
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
            try
            {
                if (!CheckoutManager.Payment.IsReadyToInvoice())
                {
                    var logData = MultiLogger.GetBaseLoggingData();
                    var invalidOp = new InvalidOperationException("Cannot apply discount when CheckoutManager is not ready to invoice");
                    MultiLogHelper.Error<CheckoutDiscountControllerBase<TDiscountModel, TLineItemModel>>("Cannot apply discount", invalidOp, logData);
                }

                var redeemResult = CheckoutManager.Offer.RedeemCouponOffer(model.OfferCode);
                var resultModel = CheckoutDiscountModelModelFactory.Create(redeemResult);

                return HandleApplyDiscountSuccess(resultModel);
            }
            catch (Exception ex)
            {
                model.OfferCode = string.Empty;
                return HandleApplyDiscountException(model, ex);
            }
        }

        /// <summary>
        /// Removes a discount code.
        /// </summary>
        /// <param name="sku">
        /// The line item SKU of the discount <see cref="ILineItem"/>
        /// </param>
        /// <param name="redirectId">
        /// The Umbraco content Id for redirection.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <remarks>
        /// We have to use the 'SKU' for the line item here since the invoice at this point has
        /// only been created (not saved) so we can't rely on line item keys (which will be Guid.Empty)
        /// </remarks>
        [HttpGet]
        public virtual ActionResult RemoveDiscount(string sku, int redirectId)
        {
            try
            {
                var invoice = CheckoutManager.Payment.PrepareInvoice();
                var lineItem = invoice.Items.FirstOrDefault(x => x.Sku == sku);
                if (lineItem == null || lineItem.LineItemType != Core.LineItemType.Discount)
                {
                    var invalidOp = new NullReferenceException("No discount line items found");
                    throw invalidOp;
                }

                var offer = lineItem.ExtendedData.GetOfferSettingsDisplay();
                CheckoutManager.Offer.RemoveOfferCode(offer.OfferCode);
                return HandleRemoveDiscountSuccess(lineItem, redirectId);
            }
            catch (Exception ex)
            {
                return HandleRemoveDiscountException(sku, redirectId, ex);
            }
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
            if (!CheckoutManager.Payment.IsReadyToInvoice()) return InvalidCheckoutStagePartial();

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
            if (model.ViewData.Success)
            {
                return RedirectToCurrentUmbracoPage();
            }

            ViewData["MerchelloViewData"] = model.ViewData;
            return CurrentUmbracoPage();
        }

        /// <summary>
        /// Handles an apply discount operation exception.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
        /// </param>
        /// <param name="ex">
        /// The <see cref="Exception"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// The <see cref="Exception"/> to be handled
        /// </exception>
        protected virtual ActionResult HandleApplyDiscountException(TDiscountModel model, Exception ex)
        {
            throw ex;
        }

        /// <summary>
        /// Handles the successful remove discount operation.
        /// </summary>
        /// <param name="lineItem">
        /// The discount <see cref="ILineItem"/>.
        /// </param>
        /// <param name="redirectId">
        /// The Umbraco content Id for redirection.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult HandleRemoveDiscountSuccess(ILineItem lineItem, int redirectId)
        {
            return RedirectToUmbracoPage(redirectId);
        }

        /// <summary>
        /// Handles the exception in the remove discount operation.
        /// </summary>
        /// <param name="sku">
        /// The line item SKU for the discount line item.
        /// </param>
        /// <param name="contentRedirectId">
        /// The Umbraco content Id for redirection.
        /// </param>
        /// <param name="ex">
        /// The <see cref="Exception"/>
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult HandleRemoveDiscountException(string sku, int contentRedirectId, Exception ex)
        {
            var logData = MultiLogger.GetBaseLoggingData();
            logData.AddCategory("Controllers");
            MultiLogHelper.WarnWithException<CheckoutDiscountControllerBase<TDiscountModel, TLineItemModel>>("Failed to remove discount", ex, logData);
            return RedirectToUmbracoPage(contentRedirectId);
        }
    }
}