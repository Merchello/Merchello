namespace Merchello.Implementation.Controllers
{
    using Merchello.Implementation.Controllers.Base;
    using Merchello.Implementation.Factories;
    using Merchello.Implementation.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default (generic) basket controller.
    /// </summary>
    [PluginController("Merchello")]
    public class DefaultBasketController : BasketControllerBase<BasketModel, BasketItemModel, AddItemModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBasketController"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor allows you to inject your custom model factory overrides so that you can
        /// extended the various model interfaces with site specific models.  In this case, we have overridden 
        /// the BasketModelFactory and the AddItemModelFactory.  The BasketItemExtendedDataFactory has not been overridden.
        /// 
        /// Views rendered by this controller are placed in "/Views/DefaultBasket/" and correspond to the method name.  
        /// 
        /// e.g.  the "AddToBasketForm" corresponds the the AddToBasketForm method in BasketControllerBase. 
        /// 
        /// This is just an generic MVC pattern and nothing to do with Umbraco
        /// </remarks>
        public DefaultBasketController()
            : base(
                  new DefaultBasketModelFactory(),
                  new DefaultAddItemModelFactory(),
                  new BasketItemExtendedDataFactory<AddItemModel>())
        {
        }
    }
}