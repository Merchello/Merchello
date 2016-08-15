namespace Merchello.Web.Store.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Web.Controllers;
    using Merchello.Web.Store.Factories;
    using Merchello.Web.Store.Models;
    using Merchello.Web.Store.Models.Async;

    using Umbraco.Web.Mvc;

    /// <summary>
    ///  A controller responsible for rendering a wish list and handling wish list operations.
    /// </summary>
    [PluginController("Merchello")]
    [Authorize]
    public class StoreWishListController : WishListControllerBase<StoreItemCacheModel, StoreLineItemModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreWishListController"/> class.
        /// </summary>
        public StoreWishListController()
            : this(new ItemCacheModelFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreWishListController"/> class.
        /// </summary>
        /// <param name="itemCacheModelFactory">
        /// The <see cref="ItemCacheModelFactory"/>.
        /// </param>
        public StoreWishListController(ItemCacheModelFactory itemCacheModelFactory)
            : base(itemCacheModelFactory)
        {
        }

        /// <summary>
        /// Overrides the success handling for wish list updates to return a JSON response.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected override ActionResult HandleUpdateWishListSuccess(StoreItemCacheModel model)
        {
            if (Request.IsAjaxRequest())
            {
                // in case of Async call we need to construct the response
                var resp = new UpdateQuantityAsyncResponse { Success = true };
                try
                {
                    resp.AddUpdatedItems(this.WishList.Items);
                    resp.FormattedTotal = this.WishList.TotalWishListPrice.AsFormattedCurrency();
                    resp.ItemCount = this.GetWishListItemCountForDisplay();
                    return this.Json(resp);
                }
                catch (Exception ex)
                {
                    resp.Success = false;
                    resp.Messages.Add(ex.Message);
                    return this.Json(resp);
                }
            }
            return base.HandleUpdateWishListSuccess(model);
        }
    }
}