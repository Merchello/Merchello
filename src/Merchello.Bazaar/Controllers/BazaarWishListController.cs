namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar wish list controller.
    /// </summary>
    [PluginController("Bazaar")]
    [Authorize]
    public class BazaarWishListController : RenderControllerBase
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
            var viewModel = ViewModelFactory.CreateWishList(model); 

            return this.View(viewModel.ThemeViewPath("WishList"), viewModel);
        }
    }
}