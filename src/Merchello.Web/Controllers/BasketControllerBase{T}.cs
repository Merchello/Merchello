namespace Merchello.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Web;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Models.Ui.Async;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Mvc;
    using Merchello.Web.Workflow;

    using Newtonsoft.Json;

	using Umbraco.Core;

    /// <summary>
    /// A base controller used for Basket implementations.
    /// </summary>
    /// <typeparam name="TBasketModel">
    /// The type of <see cref="IBasketModel{TBasketItemModel}"/>
    /// </typeparam>
    /// <typeparam name="TBasketItemModel">
    /// The type of the basket item model
    /// </typeparam>
    /// <typeparam name="TAddItem">
    /// The type of <see cref="IAddItemModel"/>
    /// </typeparam>
    public abstract class BasketControllerBase<TBasketModel, TBasketItemModel, TAddItem> : MerchelloUIControllerBase
        where TBasketItemModel : class, ILineItemModel, new()
        where TBasketModel : class, IBasketModel<TBasketItemModel>, new()
        where TAddItem : class, IAddItemModel, new()
    {
        /// <summary>
        /// The factory responsible for building the <see cref="IBasketModel{TBasketItemModel}"/>.
        /// </summary>
        private readonly BasketModelFactory<TBasketModel, TBasketItemModel> _basketModelFactory;

        /// <summary>
        /// The factory responsible for building the <see cref="IAddItemModel"/>s.
        /// </summary>
        private readonly AddItemModelFactory<TAddItem> _addItemFactory;

        /// <summary>
        /// The factory responsible for building <see cref="ExtendedDataCollection"/>s when adding items to the basket.
        /// </summary>
        private readonly BasketItemExtendedDataFactory<TAddItem> _addItemExtendedDataFactory;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketControllerBase{TBasketModel,TBasketItemModel,TAddItem}"/> class. 
        /// </summary>
        protected BasketControllerBase()
            : this(
                  new BasketItemExtendedDataFactory<TAddItem>(),
                  new AddItemModelFactory<TAddItem>(),
                  new BasketModelFactory<TBasketModel, TBasketItemModel>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketControllerBase{TBasketModel,TBasketItemModel,TAddItem}"/> class.
        /// </summary>
        /// <param name="addItemExtendedDataFactory">
        /// The <see cref="BasketItemExtendedDataFactory{TAddItemModel}"/>.
        /// </param>
        protected BasketControllerBase(BasketItemExtendedDataFactory<TAddItem> addItemExtendedDataFactory)
            : this(
                    addItemExtendedDataFactory,
                    new AddItemModelFactory<TAddItem>(),
                    new BasketModelFactory<TBasketModel, TBasketItemModel>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketControllerBase{TBasketModel,TBasketItemModel,TAddItem}"/> class. 
        /// </summary>
        /// <param name="addItemExtendedDataFactory">
        /// The <see cref="BasketItemExtendedDataFactory{TAddItemModel}"/>.
        /// </param>
        /// <param name="addItemFactory">
        /// The <see cref="AddItemModelFactory{TAddItemModel}"/>
        /// </param>
        /// <param name="basketModelFactory">
        /// The <see cref="BasketModelFactory{TBasketModel, TBasketItemModel}"/>.
        /// </param>
        protected BasketControllerBase(
            BasketItemExtendedDataFactory<TAddItem> addItemExtendedDataFactory,
            AddItemModelFactory<TAddItem> addItemFactory,
            BasketModelFactory<TBasketModel, TBasketItemModel> basketModelFactory)
        {
            Ensure.ParameterNotNull(basketModelFactory, "basketModelFactory");
            Ensure.ParameterNotNull(addItemFactory, "addItemFactory");
            Ensure.ParameterNotNull(addItemExtendedDataFactory, "addItemExtendedDataFactory");

            this._basketModelFactory = basketModelFactory;
            this._addItemFactory = addItemFactory;
            this._addItemExtendedDataFactory = addItemExtendedDataFactory;
        }

        #endregion

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
        [ValidateAntiForgeryToken]
        public virtual ActionResult AddBasketItem(TAddItem model)
        {
            // Instantiating the ExtendedDataCollection in this manner allows for additional values 
            // to be added in the factory OnCreate override.
            // e.g. if you need to store custom extended data values, create your own factory
            // inheriting from BasketItemExtendedDataFactory and override the "OnCreate" method to store
            // any addition values you have added to the model
            var extendedData = this._addItemExtendedDataFactory.Create(model);
            
            // We've added some data modifiers that can handle such things as including taxes in product
            // pricing.  The data modifiers can either get executed when the item is added to the basket or
            // as a result from a MerchelloHelper query - you just don't want them to execute twice.

            // In this case we want to get the product without any data modification
            var merchello = new MerchelloHelper(false);

            try
            {
                var product = merchello.Query.Product.GetByKey(model.ProductKey);

                // ensure the quantity on the model
                var quantity = model.Quantity <= 0 ? 1 : model.Quantity;

                // In the event the product has options we want to add the "variant" to the basket.
                // -- If a product that has variants is defined, the FIRST variant will be added to the cart. 
                // -- This was done so that we did not have to throw an error since the Master variant is no
                // -- longer valid for sale.
                if (model.OptionChoices != null && model.OptionChoices.Any())
                {
                    var variant = product.GetProductVariantDisplayWithAttributes(model.OptionChoices);

                    // Log the option choice for this variant in the extend data collection
                    var choiceExplainations = new Dictionary<string, string>();
                    foreach (var choice in variant.Attributes)
                    {
                        var option = product.ProductOptions.FirstOrDefault(x => x.Key == choice.OptionKey);
                        if (option != null)
                        {
                            choiceExplainations.Add(option.Name, choice.Name);
                        }
                    }

                    // store the choice explainations in the extended data collection
                    extendedData.SetValue(Core.Constants.ExtendedDataKeys.BasketItemCustomerChoice, JsonConvert.SerializeObject(choiceExplainations));

                    this.Basket.AddItem(variant, variant.Name, quantity, extendedData);
                }
                else
                {
                    this.Basket.AddItem(product, product.Name, quantity, extendedData);
                }

                this.Basket.Save();

                // If this request is not an AJAX request return the redirect
                return this.HandleAddItemSuccess(model);
            }
            catch (Exception ex)
            {
                var logData = MultiLogger.GetBaseLoggingData();
                logData.AddCategory("Controllers");
                MultiLogHelper.Error<BasketControllerBase<TBasketModel, TBasketItemModel, TAddItem>>("Failed to add item to basket", ex, logData);
                return this.HandleAddItemException(model, ex);
            }
        }


        /// <summary>
        /// Responsible for updating the quantities of items in the basket
        /// </summary>
        /// <param name="model">The <see cref="IBasketModel{TBasketItemModel}"/></param>
        /// <returns>Redirects to the current Umbraco page (generally the basket page)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult UpdateBasket(TBasketModel model)
        {
            if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();

            // The only thing that can be updated in this basket is the quantity
            foreach (var item in model.Items.Where(item => this.Basket.Items.First(x => x.Key == item.Key).Quantity != item.Quantity))
            {
                this.Basket.UpdateQuantity(item.Key, item.Quantity);
            }

            this.Basket.Save();

            return this.HandleUpdateBasketSuccess(model);
        }

        /// <summary>
        /// Removes an item from the basket.
        /// </summary>
        /// <param name="lineItemKey">
        /// The unique key (GUID) of the line item to be removed from the basket.
        /// </param>
        /// <param name="redirectId">
        /// The Umbraco content Id of the page to redirect to after removing the basket item.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public virtual ActionResult RemoveBasketItem(Guid lineItemKey, int redirectId)
        {
            this.EnsureOwner(this.Basket.Items, lineItemKey);

            // remove the item by it's pk.  
            this.Basket.RemoveItem(lineItemKey);

            this.Basket.Save();

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
        public virtual ActionResult MoveItemToWishList(Guid lineItemKey, int successRedirectId)
        {
            // Assert the customer is not anonymous
            if (this.CurrentCustomer.IsAnonymous) return this.RedirectToCurrentUmbracoPage();

            // Ensure the basket item reference is in the current customer's basket
            // e.g. it is not a reference to some other customer's basket
            this.EnsureOwner(this.Basket.Items, lineItemKey);

            // Move the item to the wish list collection
            this.Basket.MoveItemToWishList(lineItemKey);

            return this.RedirectToUmbracoPage(successRedirectId);
        }

        #region ChildActions

        /// <summary>
        /// Renders the basket partial view.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult BasketForm(string view = "")
        {
            var model = this._basketModelFactory.Create(this.Basket);
            return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
        }


        /// <summary>
        /// Responsible for rendering the Add Item Form.
        /// </summary>
        /// <param name="model">
        /// The <see cref="IProductContent"/>.
        /// </param>
        /// <param name="quantity">
        /// The quantity to be added
        /// </param>
        /// <param name="view">
        /// The name of the view to render.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult AddProductToBasketForm(IProductContent model, int quantity = 1, string view = "AddToBasketForm")
        {
            var addItem = this._addItemFactory.Create(model, quantity);
            return this.AddToBasketForm(addItem, view);
        }

        /// <summary>
        /// Responsible for rendering the Add Item Form.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ProductDisplay"/>.
        /// </param>
        /// <param name="quantity">
        /// The quantity to be added
        /// </param>
        /// <param name="view">
        /// The name of the view to render.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult AddProductDisplayToBasketForm(ProductDisplay model, int quantity = 1, string view = "AddToBasketForm")
        {
            var addItem = this._addItemFactory.Create(model, quantity);
            return this.AddToBasketForm(addItem, view);
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
            return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
        }

        #endregion

        #region Operation Handlers

        /// <summary>
        /// Handles the successful basket update.
        /// </summary>
        /// <param name="model">
        /// The <see cref="IBasketModel{TBasketItemModel}"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <remarks>
        /// Allows for customization of the redirection after a custom update basket operation
        /// </remarks>
        protected virtual ActionResult HandleUpdateBasketSuccess(TBasketModel model)
        {
            return this.RedirectToCurrentUmbracoPage();
        }

        /// <summary>
        /// Handles a basket update exception
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
        protected virtual ActionResult HandleUpdateBasketException(TBasketModel model, Exception ex)
        {
            throw ex;
        }

        /// <summary>
        /// Handles a successful add item operation.
        /// </summary>
        /// <param name="model">
        /// The <see cref="IAddItemModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult HandleAddItemSuccess(TAddItem model)
        {
            return this.CurrentUmbracoPage();
        }

        /// <summary>
        /// Handles an add item operation exception.
        /// </summary>
        /// <param name="model">
        /// The <see cref="IAddItemModel"/>.
        /// </param>
        /// <param name="ex">
        /// The <see cref="Exception"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult HandleAddItemException(TAddItem model, Exception ex)
        {
            var logData = MultiLogger.GetBaseLoggingData();
            MultiLogHelper.Error<BasketControllerBase<TBasketModel, TBasketItemModel, TAddItem>>("Failed to add item to the basket", ex, logData);
            throw ex;
        }

        #endregion

        /// <summary>
        /// Gets the total basket count.
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
        protected virtual int GetBasketItemCountForDisplay()
        {
            return this.Basket.TotalQuantityCount;
        }
    }
}