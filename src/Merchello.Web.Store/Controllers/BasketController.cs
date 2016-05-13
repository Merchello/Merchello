namespace Merchello.Web.Store.Controllers
{
    using Merchello.QuickMart.Factories;
    using Merchello.Web.Controllers;
    using Merchello.Web.Factories;
    using Merchello.Web.Store.Factories;
    using Merchello.Web.Store.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default (generic) basket controller.
    /// </summary>
    [PluginController("Merchello")]
    public class BasketController : BasketControllerBase<BasketModel, StoreLineItemModel, AddItemModel>
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
                  new BasketItemExtendedDataFactory<AddItemModel>())
        {
        }
    }
}