namespace Merchello.Bazaar.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Core.Models;
    using Merchello.Web;
    using Merchello.Web.Workflow;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar wish list controller.
    /// </summary>
    [PluginController("Bazaar")]
    [Authorize]
    public partial class BazaarWishListController : BazaarControllerBase
    {
        /// <summary>
        /// The <see cref="IWishList"/>.
        /// </summary>
        private readonly IWishList _wishList;

        /// <summary>
        /// Initializes a new instance of the <see cref="BazaarWishListController"/> class.
        /// </summary>
        public BazaarWishListController()
        {
            if (CurrentCustomer != null) _wishList = ((ICustomer)CurrentCustomer).WishList();
        }

        /// <summary>
        /// The index <see cref="ActionResult"/>.
        /// </summary>
        /// <param name="model">
        /// The current render model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public override ActionResult Index(RenderModel model)
        {
            var viewModel = ViewModelFactory.CreateWishList(model); 

            return this.View(viewModel.ThemeViewPath("WishList"), viewModel);
        }

        /// <summary>
        /// Responsible for adding a product to the basket.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult AddToWishList(AddItemModel model)
        {
            var extendedData = new ExtendedDataCollection();
            extendedData.SetValue("umbracoContentId", model.ContentId.ToString(CultureInfo.InvariantCulture));

            var product = this.MerchelloServices.ProductService.GetByKey(model.Product.Key);
            if (model.OptionChoices != null && model.OptionChoices.Any())
            {
                var variant = this.MerchelloServices.ProductVariantService.GetProductVariantWithAttributes(product, model.OptionChoices);
                _wishList.AddItem(variant, variant.Name, 1, extendedData);
            }
            else
            {
                _wishList.AddItem(product, product.Name, 1, extendedData);
            }

            _wishList.Save();

            return this.RedirectToUmbracoPage(model.WishListPageId);
        }


        /// <summary>
        /// Responsible for updating the quantities of items in the wish list
        /// </summary>
        /// <param name="model">The <see cref="IEnumerable{T}"/></param>
        /// <returns>Redirects to the current Umbraco page (the wish list page)</returns>
        [HttpPost]
        public ActionResult UpdateWishList(WishListTableModel model)
        {
            if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();

            // The only thing that can be updated in this basket is the quantity
            foreach (var item in model.Items.Where(item => _wishList.Items.First(x => x.Key == item.Key).Quantity != item.Quantity))
            {
                _wishList.UpdateQuantity(item.Key, item.Quantity);
            }

            _wishList.Save();

            return this.CurrentUmbracoPage();
        }


        /// <summary>
        /// Removes an item from the wish list.
        /// </summary>
        /// <param name="lineItemKey">
        /// The line item key.
        /// </param>
        /// <param name="wishListPageId">
        /// The wish list page id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Throws an exception if an attempt is made to remove an item from another customers wish list id
        /// </exception>
        [HttpGet]
        public ActionResult RemoveItemFromWishList(Guid lineItemKey, int wishListPageId)
        {
            EnsureOwner(_wishList.Items, lineItemKey);

            // remove the item by it's pk.  
            _wishList.RemoveItem(lineItemKey);

            _wishList.Save();

            return this.RedirectToUmbracoPage(wishListPageId);
        }

        /// <summary>
        /// Moves an item from the WishList to the Basket.
        /// </summary>
        /// <param name="lineItemKey">
        /// The line item key.
        /// </param>
        /// <param name="basketPageId">
        /// The basket page id.
        /// </param>
        /// <param name="wishListPageId">
        /// The wish list page id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public ActionResult MoveItemToBasket(Guid lineItemKey, int basketPageId, int wishListPageId)
        {
            if (CurrentCustomer.IsAnonymous) return this.RedirectToUmbracoPage(basketPageId);

            EnsureOwner(_wishList.Items, lineItemKey);

            _wishList.MoveItemToBasket(lineItemKey);

            return _wishList.IsEmpty ? RedirectToUmbracoPage(basketPageId) : RedirectToUmbracoPage(wishListPageId);
        }
    }
}