namespace Merchello.Implementation.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Core.Models;
    using Merchello.Implementation.Factories;
    using Merchello.Implementation.Models;
    using Merchello.Web;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Mvc;
    using Merchello.Web.Workflow;

    using Umbraco.Core;

    /// <summary>
    /// A base controller used for Basket implementations.
    /// </summary>
    /// <typeparam name="TBasket">
    /// The type of <see cref="IBasketModel"/>
    /// </typeparam>
    /// <typeparam name="TAddItem">
    /// The type of <see cref="IAddItemModel"/>
    /// </typeparam>
    public abstract class BasketControllerBase<TBasket, TAddItem> : MerchelloSurfaceController, IViewRendererController
        where TBasket : class, IBasketModel, new()
        where TAddItem : class, IAddItemModel, new()
    {
        /// <summary>
        /// The factory responsible for building <see cref="ExtendedDataCollection"/>s when adding items to the basket.
        /// </summary>
        private readonly BasketItemExtendedDataFactory<TAddItem> _addItemExtendedDataFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketControllerBase{TBasket,TAddItem}"/> class.
        /// </summary>
        protected BasketControllerBase()
            : this(new BasketItemExtendedDataFactory<TAddItem>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketControllerBase{TBasket,TAddItem}"/> class.
        /// </summary>
        /// <param name="addItemExtendedDataFactory">
        /// The <see cref="BasketItemExtendedDataFactory{TAddItemModel}"/>.
        /// </param>
        protected BasketControllerBase(BasketItemExtendedDataFactory<TAddItem> addItemExtendedDataFactory)
        {
            Mandate.ParameterNotNull(addItemExtendedDataFactory, "addItemExtendedDataFactor");

            _addItemExtendedDataFactory = addItemExtendedDataFactory;
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
        public virtual ActionResult AddBasketItem(TAddItem model)
        {
            // Instantiating the ExtendedDataCollection in this manner allows for additional values 
            // to be added in the factory OnCreate override.
            // e.g. if you need to store custom extended data values, create your own factory
            // inheriting from BasketItemExtendedDataFactory and override the "OnCreate" method to store
            // any addition values you have added to the model
            var extendedData = _addItemExtendedDataFactory.Build(model);
            
            // We've added some data modifiers that can handle such things as including taxes in product
            // pricing.  The data modifiers can either get executed when the item is added to the basket or
            // as a result from a MerchelloHelper query - you just don't want them to execute twice.

            // In this case we want to get the product without any data modification
            var merchello = new MerchelloHelper(false);

            var product = merchello.Query.Product.GetByKey(model.ProductKey);

            // In the event the product has options we want to add the "variant" to the basket.
            // -- If a product that has variants is defined, the FIRST variant will be added to the cart. 
            // -- This was done so that we did not have to throw an error since the Master variant is no
            // -- longer valid for sale.
            if (model.OptionChoices != null && model.OptionChoices.Any())
            {
                var variant = product.GetProductVariantDisplayWithAttributes(model.OptionChoices);
                this.Basket.AddItem(variant, variant.Name, 1, extendedData);
            }
            else
            {
                this.Basket.AddItem(product, product.Name, 1, extendedData);
            }

            this.Basket.Save();

            return RedirectAddItemSuccess(model);
        }


        /// <summary>
        /// Responsible for updating the quantities of items in the basket
        /// </summary>
        /// <param name="model">The <see cref="TBasket"/></param>
        /// <returns>Redirects to the current Umbraco page (generally the basket page)</returns>
        [HttpPost]
        public virtual ActionResult UpdateBasket(TBasket model)
        {
            if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();

            // The only thing that can be updated in this basket is the quantity
            foreach (var item in model.Items.Where(item => this.Basket.Items.First(x => x.Key == item.Key).Quantity != item.Quantity))
            {
                this.Basket.UpdateQuantity(item.Key, item.Quantity);
            }

            this.Basket.Save();

            return RedirectUpdateBasketSuccess(model);
        }

        /// <summary>
        /// Removes an item from the basket.
        /// </summary>
        /// <param name="lineItemKey">
        /// The unique key (GUID) of the line item to be removed from the basket.
        /// </param>
        /// <param name="successPageId">
        /// The Umbraco content id of the page to redirect to after the basket item has been removed.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public ActionResult RemoveBasketItem(Guid lineItemKey, int successPageId)
        {
            EnsureOwner(Basket.Items, lineItemKey);

            // remove the item by it's pk.  
            this.Basket.RemoveItem(lineItemKey);

            this.Basket.Save();

            return this.RedirectToUmbracoPage(successPageId);
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
        public virtual ActionResult MoveItemToWishList(Guid lineItemKey, int successRedirectId)
        {
            // Assert the customer is not anonymous
            if (CurrentCustomer.IsAnonymous) return RedirectToCurrentUmbracoPage();

            // Ensure the basket item reference is in the current customer's basket
            // e.g. it is not a reference to some other customer's basket
            EnsureOwner(Basket.Items, lineItemKey);

            // Move the item to the wish list collection
            Basket.MoveItemToWishList(lineItemKey);

            return RedirectToUmbracoPage(successRedirectId);
        }

        #region ChildActions

        /// <summary>
        /// Renders the basket partial view.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public virtual ActionResult BasketForm(TBasket model)
        {
            return PartialView(model);
        }

        /// <summary>
        /// Responsible for rendering the Add.
        /// </summary>
        /// <param name="model">
        /// The <see cref="IProductContent"/>.
        /// </param>
        /// <param name="view">
        /// The name of the view to render.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult AddProductToBasketForm(IProductContent model, string view = "AddToBasketForm")
        {
            var addItem = MapProductContentToAddItemModel(model);
            return AddToBasketForm(addItem, view);
        }

        /// <summary>
        /// Renders the add to basket form.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="view">
        /// The name of the view to render.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult AddToBasketForm(TAddItem model, string view = "")
        {
            return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
        }

        #endregion

        #region Success Redirects

        /// <summary>
        /// Handles the redirection after a successful basket update.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <remarks>
        /// Allows for customization of the redirection after a custom update basket operation
        /// </remarks>
        protected virtual ActionResult RedirectUpdateBasketSuccess(TBasket model)
        {
            return RedirectToCurrentUmbracoPage();
        }

        /// <summary>
        /// The redirect add item success.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected abstract ActionResult RedirectAddItemSuccess(TAddItem model);

        #endregion

        /// <summary>
        /// Maps <see cref="IProductContent"/> to <see cref="TAddItem"/>.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <returns>
        /// The mapped <see cref="TAddItem"/> object.
        /// </returns>
        protected abstract TAddItem MapProductContentToAddItemModel(IProductContent product);
    }
}