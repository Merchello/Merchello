namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The controller responsible for rendering the Basket Page.
    /// </summary>
    [PluginController("Bazaar")]
    public class BazaarBasketController : RenderControllerBase
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
    }
}