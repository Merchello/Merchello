namespace Merchello.QuickMart.Controllers
{
    using Merchello.QuickMart.Factories;
    using Merchello.QuickMart.Models;
    using Merchello.Web.Controllers;
    using Merchello.Web.Factories;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default (generic) basket controller.
    /// </summary>
    [PluginController("QuickMart")]
    public class BasketController : BasketControllerBase<BasketModel, BasketItemModel, AddItemModel>
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
                  new QuickMartBasketModelFactory(),
                  new QuickMartAddItemModelFactory(),
                  new BasketItemExtendedDataFactory<AddItemModel>())
        {
        }
    }
}