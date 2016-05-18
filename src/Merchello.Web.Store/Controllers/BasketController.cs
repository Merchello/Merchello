namespace Merchello.Web.Store.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.QuickMart.Factories;
    using Merchello.Web.Controllers;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui.Async;
    using Merchello.Web.Store.Factories;
    using Merchello.Web.Store.Models;
    using Merchello.Web.Store.Models.Async;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default (generic) basket controller.
    /// </summary>
    [PluginController("Merchello")]
    public class BasketController : BasketControllerBase<StoreBasketModel, StoreLineItemModel, StoreAddItemModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasketController"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor allows you to inject your custom model factory overrides so that you can
        /// extended the various model interfaces with site specific models.  In this case, we have overridden 
        /// the BasketModelFactory and the AddItemModelFactory.  The BasketItemExtendedDataFactory has not been overridden.
        /// 
        /// Views rendered by this controller are placed in "/Views/QuickMartBasket/" and correspond to the method name.  
        /// 
        /// e.g.  the "AddToBasketForm" corresponds the the AddToBasketForm method in BasketControllerBase. 
        /// 
        /// This is just an generic MVC pattern and nothing to do with Umbraco
        /// </remarks>
        public BasketController()
            : base(
                  new BasketModelFactory(),
                  new AddItemModelFactory(),
                  new BasketItemExtendedDataFactory<StoreAddItemModel>())
        {
        }

        /// <summary>
        /// Handles the successful basket update.
        /// </summary>
        /// <param name="model">
        /// The <see cref="StoreBasketModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <remarks>
        /// Customization of the handling of an add item success
        /// </remarks>
        protected override ActionResult HandleAddItemSuccess(StoreAddItemModel model)
        {
            if (Request.IsAjaxRequest())
            {
                // Construct the response object to return
                var resp = new AddItemAsyncResponse
                    {
                        Success = true,
                        BasketItemCount = this.GetBasketItemCountForDisplay()
                    };

                return this.Json(resp);
            }

            return base.HandleAddItemSuccess(model);
        }

        /// <summary>
        /// Handles an add item operation exception.
        /// </summary>
        /// <param name="model">
        /// The <see cref="StoreAddItemModel"/>.
        /// </param>
        /// <param name="ex">
        /// The <see cref="Exception"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected override ActionResult HandleAddItemException(StoreAddItemModel model, Exception ex)
        {
            if (Request.IsAjaxRequest())
            {
                // in case of Async call we need to construct the response
                var resp = new AddItemAsyncResponse { Success = false, Messages = { ex.Message } };
                return this.Json(resp);
            }

            return base.HandleAddItemException(model, ex);
        }

        /// <summary>
        /// Handles the successful basket update.
        /// </summary>
        /// <param name="model">
        /// The <see cref="StoreBasketModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <remarks>
        /// Handles the customization of the redirection after a custom update basket operation
        /// </remarks>
        protected override ActionResult HandleUpdateBasketSuccess(StoreBasketModel model)
        {
            if (Request.IsAjaxRequest())
            {
                // in case of Async call we need to construct the response
                var resp = new UpdateQuantityAsyncResponse { Success = true };
                try
                {
                    resp.AddUpdatedItems(this.Basket.Items);
                    resp.FormattedTotal = this.Basket.TotalBasketPrice.AsFormattedCurrency();
                    resp.BasketItemCount = this.GetBasketItemCountForDisplay();
                    return this.Json(resp);
                }
                catch (Exception ex)
                {
                    resp.Success = false;
                    resp.Messages.Add(ex.Message);
                    return this.Json(resp);
                }
            }

            return base.HandleUpdateBasketSuccess(model);
        }
    }
}