namespace Merchello.Bazaar.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core.Models;
    using Merchello.Web;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Workflow;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The controller responsible for rendering the Basket Page.
    /// </summary>
    [PluginController("Bazaar")]
    public partial class BazaarBasketController : BazaarControllerBase
    {
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
            var viewModel = ViewModelFactory.CreateBasket(model, Basket);
              
            return View(viewModel.ThemeViewPath("Basket"), viewModel);
        }

        /// <summary>
        /// Responsible for rendering the add to basket partial view.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ProductModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public ActionResult RenderAddToBasket(ProductModel model)
        {
            var addItemModel = new AddItemModel()
            {
                Product = model.ProductData,
                ContentId = model.Id,
                BasketPageId = BazaarContentHelper.GetBasketContent().Id,
                ShowWishList = model.ShowWishList,
                WishListPageId = BazaarContentHelper.GetWishListContent().Id,
                Currency = model.Currency
            };

            return RenderAddItem(addItemModel);
        }

        /// <summary>
        /// The render add to basket.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public ActionResult RenderAddItem(AddItemModel model)
        {
            var theme = BazaarContentHelper.GetStoreTheme();
            return this.PartialView(PathHelper.GetThemePartialViewPath(theme, "AddToCart"), model);
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
        public ActionResult AddToBasket(AddItemModel model)
        {

            // This is for legacy implementations of the Bazaar.  Using IProductContent we don't need this anymore.
            var extendedData = new ExtendedDataCollection();
            extendedData.SetValue("umbracoContentId", model.ContentId.ToString(CultureInfo.InvariantCulture));           

            // We've added some data modifiers that can handle such things as including taxes in product
            // pricing.  The data modifiers can either get executed when the item is added to the basket or
            // as a result from a MerchelloHelper query - you just don't want them to execute twice.

            // Calls directly to the ProductService are not modified
            // var product = this.MerchelloServices.ProductService.GetByKey(model.Product.Key);

            // Calls to using the MerchelloHelper WILL be modified
            // var merchello = new MerchelloHelper();
            //
            // if you want to do this you should tell the basket not to modify the data again when adding the item
            //this.Basket.EnableDataModifiers = false;

            // In this case we want to get the product without any data modification
            var merchello = new MerchelloHelper(false);

            var product = merchello.Query.Product.GetByKey(model.Product.Key);

            // In the event the product has options we want to add the "variant" to the basket.
            // -- If a product that has variants is defined, the FIRST variant will be added to the cart. 
            // -- This was done so that we did not have to throw an error since the Master variant is no
            // -- longer valid for sale.
            if (model.OptionChoices != null && model.OptionChoices.Any())
            {
                // NEW in 1.9.1
                // ProductDisplay and ProductVariantDisplay classes can be added directly to the Basket
                // so you don't have to query the service.
                var variant = product.GetProductVariantDisplayWithAttributes(model.OptionChoices);
                this.Basket.AddItem(variant, variant.Name, 1, extendedData);
            }
            else
            {
                this.Basket.AddItem(product, product.Name, 1, extendedData);
            }

            this.Basket.Save();

            return this.RedirectToUmbracoPage(model.BasketPageId);
        }


        /// <summary>
        /// Responsible for updating the quantities of items in the basket
        /// </summary>
        /// <param name="model">The <see cref="IEnumerable{T}"/></param>
        /// <returns>Redirects to the current Umbraco page (the basket page)</returns>
        [HttpPost]
        public ActionResult UpdateBasket(BasketTableModel model)
        {
            if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();

            // The only thing that can be updated in this basket is the quantity
            foreach (var item in model.Items.Where(item => this.Basket.Items.First(x => x.Key == item.Key).Quantity != item.Quantity))
            {
                this.Basket.UpdateQuantity(item.Key, item.Quantity);
            }

            this.Basket.Save();

            return this.CurrentUmbracoPage();
        }


        /// <summary>
        /// Removes an item from the basket.
        /// </summary>
        /// <param name="lineItemKey">
        /// The line item key.
        /// </param>
        /// <param name="basketPageId">
        /// The basket page id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Throws an exception if an attempt is made to remove an item from another customers basket
        /// </exception>
        [HttpGet]
        public ActionResult RemoveItemFromBasket(Guid lineItemKey, int basketPageId)
        {
            EnsureOwner(Basket.Items, lineItemKey);

            // remove the item by it's pk.  
            this.Basket.RemoveItem(lineItemKey);

            this.Basket.Save();

            return this.RedirectToUmbracoPage(basketPageId);
        }

        /// <summary>
        /// Moves an item from the Basket to the WishList.
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
        public ActionResult MoveItemToWishList(Guid lineItemKey, int basketPageId, int wishListPageId)
        {
            if (CurrentCustomer.IsAnonymous) return this.RedirectToUmbracoPage(basketPageId);

            EnsureOwner(Basket.Items, lineItemKey);

            Basket.MoveItemToWishList(lineItemKey);

            return RedirectToUmbracoPage(wishListPageId);
        }
    }
}