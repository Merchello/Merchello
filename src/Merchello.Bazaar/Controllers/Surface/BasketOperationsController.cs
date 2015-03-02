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
    using Merchello.Web.Workflow;

    using Umbraco.Core.Logging;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// A <see cref="SurfaceController"/> responsible for store workflow operations.
    /// </summary>
    [PluginController("Bazaar")]
    public class BasketOperationsController : SurfaceControllerBase
    {
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
                BasketPageId = model.BasketPage.Id,
                ShowWishList = model.ShowWishList,
                WishListPageId = model.WishListPage.Id,
                Currency = model.Currency
            };

            return this.PartialView(model.ThemePartialViewPath("AddToCart"), addItemModel);
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
            // This is an example of using the ExtendedDataCollection to add some custom functionality.
            // In this case, we are creating a direct reference to the content (Product Detail Page) so
            // that we can provide a link, thumbnail and description in the cart per this design.  In other 
            // designs, there may not be thumbnails or descriptions and the link could be to a completely
            // different website.
            var extendedData = new ExtendedDataCollection();
            extendedData.SetValue("umbracoContentId", model.ContentId.ToString(CultureInfo.InvariantCulture));

            var product = this.MerchelloServices.ProductService.GetByKey(model.Product.Key);

            // In the event the product has options we want to add the "variant" to the basket.
            // -- If a product that has variants is defined, the FIRST variant will be added to the cart. 
            // -- This was done so that we did not have to throw an error since the Master variant is no
            // -- longer valid for sale.
            if (model.OptionChoices != null && model.OptionChoices.Any())
            {
                var variant = this.MerchelloServices.ProductVariantService.GetProductVariantWithAttributes(product, model.OptionChoices);
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