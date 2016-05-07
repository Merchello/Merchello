namespace Merchello.Implementation.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Implementation.Default.Models;
    using Merchello.Implementation.Factories;
    using Merchello.Implementation.Models;
    using Merchello.Implementation.Models.Async;
    using Merchello.Web;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Mvc;
    using Merchello.Web.Workflow;

    using Newtonsoft.Json;

    using Umbraco.Core;

    /// <summary>
    /// A base controller used for Basket implementations.
    /// </summary>
    /// <typeparam name="TBasket">
    /// The type of <see cref="IBasketModel{TBasketItemModel}"/>
    /// </typeparam>
    /// <typeparam name="TBasketItemModel">
    /// The type of the basket item model
    /// </typeparam>
    /// <typeparam name="TAddItem">
    /// The type of <see cref="IAddItemModel"/>
    /// </typeparam>
    public abstract class BasketControllerBase<TBasket, TBasketItemModel, TAddItem> : MerchelloSurfaceController, IBasketViewRenderer
        where TBasketItemModel : class, IBasketItemModel, new()
        where TBasket : class, IBasketModel<TBasketItemModel>, new()
        where TAddItem : class, IAddItemModel, new()
    {
        /// <summary>
        /// The factory responsible for building <see cref="ExtendedDataCollection"/>s when adding items to the basket.
        /// </summary>
        private readonly BasketItemExtendedDataFactory<TAddItem> _addItemExtendedDataFactory;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BasketControllerBase{TBasket,TBasketItemModel,TAddItem}"/> class. 
        /// </summary>
        protected BasketControllerBase()
            : this(new BasketItemExtendedDataFactory<TAddItem>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketControllerBase{TBasket,TBasketItemModel,TAddItem}"/> class. 
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
                extendedData.SetValue(Implementation.Constants.ExtendedDataKeys.BasketItemCustomerChoice, JsonConvert.SerializeObject(choiceExplainations));

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

            // in case of Async call we need to construct the response
            var resp = new UpdateQuantityAsyncResponse<TBasketItemModel> { Success = true };

            // The only thing that can be updated in this basket is the quantity
            foreach (var item in model.Items.Where(item => this.Basket.Items.First(x => x.Key == item.Key).Quantity != item.Quantity))
            {
                this.Basket.UpdateQuantity(item.Key, item.Quantity);
                resp.UpdatedItems.Add(item);
            }

            this.Basket.Save();

            if (Request.IsAjaxRequest())
            {
                try
                {
                    resp.FormattedTotal = Basket.TotalBasketPrice.AsFormattedCurrency();
                    return Json(resp);
                }
                catch (Exception ex)
                {
                    resp.Success = false;
                    resp.ErrorMessages.Add(ex.Message);
                    return Json(resp);
                }
               
            }

            return RedirectUpdateBasketSuccess(model);
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
        public ActionResult RemoveBasketItem(Guid lineItemKey, int redirectId)
        {
            EnsureOwner(Basket.Items, lineItemKey);

            // remove the item by it's pk.  
            this.Basket.RemoveItem(lineItemKey);

            this.Basket.Save();

            return RedirectToUmbracoPage(redirectId);
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
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public virtual ActionResult BasketForm()
        {
            var model = MapBasketToBasketModel(CurrentCustomer.Basket());
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
        /// Maps the Merchello Basket item to the implementation basket model.
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <returns>
        /// The <see cref="TBasket"/>.
        /// </returns>
        protected virtual TBasket MapBasketToBasketModel(IBasket basket)
        {
            var merchello = new MerchelloHelper();

            var items =
                basket.Items.Select(
                    basketItem =>
                    new BasketItemModel
                        {
                            Key = basketItem.Key,
                            Name = basketItem.Name,
                            Amount = basketItem.Price,
                            ProductKey = basketItem.ExtendedData.GetProductKey(),
                            Product = basketItem.ExtendedData.ContainsProductKey() && 
                                      basketItem.LineItemType == LineItemType.Product ? 
                                        GetProductContent(merchello, basketItem.ExtendedData.GetProductKey()) :
                                        null,
                            Quantity = basketItem.Quantity,
                            CustomerOptionChoices = basketItem.GetProductOptionChoicePairs()
                        }
                     
                    as TBasketItemModel);

            // TODO wish list setting
            var model = new TBasket
                {
                    WishListEnabled = false,
                    Items = items.ToArray()
                };

            return model;
        }

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

        /// <summary>
        /// Maps a <see cref="ILineItem"/> to <see cref="IBasketItemModel"/>.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <returns>
        /// The <see cref="IBasketItemModel"/>.
        /// </returns>
        protected virtual TBasketItemModel MapLineItemToBasketLineItem(ILineItem lineItem)
        {
            return new TBasketItemModel
            {
                Key = lineItem.Key,
                Name = lineItem.Name,
                ProductKey = lineItem.ExtendedData.GetProductKey(),
                Amount = lineItem.Price,
                Quantity = lineItem.Quantity
            };
        }

        /// <summary>
        /// Gets the <see cref="IProductContent"/>.
        /// </summary>
        /// <param name="merchello">
        /// The merchello.
        /// </param>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        private IProductContent GetProductContent(MerchelloHelper merchello, Guid productKey)
        {
            if (productKey.Equals(Guid.Empty)) return null;
            return merchello.TypedProductContent(productKey);
        }
    }
}