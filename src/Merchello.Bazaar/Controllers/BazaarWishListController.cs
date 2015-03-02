namespace Merchello.Bazaar.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Bazaar.Factories;
    using Merchello.Bazaar.Models;
    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core.Models;
    using Merchello.Web;

    using Umbraco.Core.Models;
    using Umbraco.Web;
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