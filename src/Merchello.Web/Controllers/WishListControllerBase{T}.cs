using Merchello.Core;

namespace Merchello.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Workflow;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// A controller responsible for rendering a wish list and handling wish list operations.
    /// </summary>
    /// <typeparam name="TItemCacheModel">
    /// The type of <see cref="IItemCacheModel{TLineItemModel}"/>
    /// </typeparam>
    /// <typeparam name="TLineItemModel">
    /// The type of <see cref="ILineItemModel"/>
    /// </typeparam>
    public abstract class WishListControllerBase<TItemCacheModel, TLineItemModel> : MerchelloUIControllerBase
        where TItemCacheModel : class, IItemCacheModel<TLineItemModel>, new()
        where TLineItemModel : class, ILineItemModel, new()
    {
        /// <summary>
        /// The factory responsible for building the <see cref="IItemCacheModel{ILineItemModel}"/> (wish list).
        /// </summary>
        private readonly ItemCacheModelFactory<TItemCacheModel, TLineItemModel> _itemCacheModelFactory;

        /// <summary>
        /// The <see cref="IWishList"/>.
        /// </summary>
        private Lazy<IWishList> _wishlist;

        /// <summary>
        /// Initializes a new instance of the <see cref="WishListControllerBase{TItemCacheModel,TLineItemModel}"/> class.
        /// </summary>
        protected WishListControllerBase()
            : this(new ItemCacheModelFactory<TItemCacheModel, TLineItemModel>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WishListControllerBase{TItemCacheModel,TLineItemModel}"/> class.
        /// </summary>
        /// <param name="itemCacheModelFactory">
        /// The item cache model factory.
        /// </param>
        protected WishListControllerBase(ItemCacheModelFactory<TItemCacheModel, TLineItemModel> itemCacheModelFactory)
        {
            Ensure.ParameterNotNull(itemCacheModelFactory, "itemCacheModelFactory");
            _itemCacheModelFactory = itemCacheModelFactory;

            this.Initialize();
        }

        /// <summary>
        /// Gets the wish list.
        /// </summary>
        protected IWishList WishList
        {
            get
            {
                return _wishlist.Value;
            }
        }

        /// <summary>
        /// Responsible for updating the quantities of items in the wish list
        /// </summary>
        /// <param name="model">The <see cref="IBasketModel{TBasketItemModel}"/></param>
        /// <returns>Redirects to the current Umbraco page (generally the basket page)</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public virtual ActionResult UpdateWishList(TItemCacheModel model)
        {
            if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();


            // The only thing that can be updated in this wish list is the quantity
            foreach (var item in model.Items.Where(item => WishList.Items.First(x => x.Key == item.Key).Quantity != item.Quantity))
            {
                WishList.UpdateQuantity(item.Key, item.Quantity);
            }

            this.Basket.Save();

            return this.HandleUpdateWishListSuccess(model);
        }

        /// <summary>
        /// Removes an item from the wish list.
        /// </summary>
        /// <param name="lineItemKey">
        /// The unique key (GUID) of the line item to be removed from the wish list.
        /// </param>
        /// <param name="redirectId">
        /// The Umbraco content Id of the page to redirect to after removing the wish list item.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        [Authorize]
        public virtual ActionResult RemoveWishListItem(Guid lineItemKey, int redirectId)
        {

            this.EnsureOwner(WishList.Items, lineItemKey);

            // remove the item by it's pk.  
            WishList.RemoveItem(lineItemKey);

            WishList.Save();

            return this.RedirectToUmbracoPage(redirectId);
        }

        /// <summary>
        /// Moves an item from the Basket to the WishList.
        /// </summary>
        /// <param name="lineItemKey">
        /// The unique key (GUID) of the line item to be moved from the basket to the wish list.
        /// </param>
        /// <param name="successRedirectId">
        /// The Umbraco content id of the page to redirect to after the basket item has been moved.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <remarks>
        /// Anonymous customers do not have wish lists, so this method will redirect to the current page if the
        /// customer has not authenticated.
        /// </remarks>
        [HttpGet]
        [Authorize]
        public virtual ActionResult MoveItemToBasket(Guid lineItemKey, int successRedirectId)
        {
            // Assert the customer is not anonymous
            if (this.CurrentCustomer.IsAnonymous) return this.RedirectToCurrentUmbracoPage();

            // Ensure the wish list item reference is in the current customer's wish list
            // e.g. it is not a reference to some other customer's wish list
            this.EnsureOwner(WishList.Items, lineItemKey);

            // Move the item to the wish list collection
            WishList.MoveItemToBasket(lineItemKey);

            return this.RedirectToUmbracoPage(successRedirectId);
        }

        /// <summary>
        /// Renders the wish list partial view.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Throws an exception if the current customer is anonymous
        /// </exception>
        [ChildActionOnly]
        [Authorize]
        public virtual ActionResult WishListForm(string view = "")
        {
            if (CurrentCustomer.IsAnonymous)
            {
                var logData = MultiLogger.GetBaseLoggingData();
                var invalidOp = new InvalidOperationException("Cannot render wish list for an anonymous customer.");
                MultiLogHelper.Error<WishListControllerBase<TItemCacheModel, TLineItemModel>>("Anonymous customers cannot have wish lists", invalidOp, logData);
                throw invalidOp;
            }


            var model = _itemCacheModelFactory.Create(((WishList)WishList).ItemCache);

            return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
        }


        #region Operation Handlers

        /// <summary>
        /// Handles the successful wish list update.
        /// </summary>
        /// <param name="model">
        /// The <see cref="IItemCacheModel{TLineItemModel}"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <remarks>
        /// Allows for customization of the redirection after a custom update wish list operation
        /// </remarks>
        protected virtual ActionResult HandleUpdateWishListSuccess(TItemCacheModel model)
        {
            return this.RedirectToCurrentUmbracoPage();
        }

        /// <summary>
        /// Handles a wish list update exception
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <remarks>
        /// Allows for customization of the redirection after a custom update basket operation
        /// </remarks>
        protected virtual ActionResult HandleUpdateWishListException(TItemCacheModel model, Exception ex)
        {
            var logData = MultiLogger.GetBaseLoggingData();
            MultiLogHelper.Error<WishListControllerBase<TItemCacheModel, TLineItemModel>>("Failed to update wish list", ex, logData);

            throw ex;
        }

        #endregion

        /// <summary>
        /// Gets the total wish list count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <remarks>
        /// This is generally used in navigations and labels.  Some implementations show the total number of line items while
        /// others show the total number of items (total sum of product quantities - default).
        /// 
        /// Method is used in Async responses to allow for easier HTML label updates 
        /// </remarks>
        protected virtual int GetWishListItemCountForDisplay()
        {
            var wishlist = ((ICustomer)CurrentCustomer).WishList();
            return wishlist.TotalQuantityCount;
        }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void Initialize()
        {
            _wishlist = new Lazy<IWishList>(() => ((ICustomer)CurrentCustomer).WishList());
        }
    }
}