using Merchello.Bazaar.Models;
using Merchello.Core.Models;

namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// Surface controller responsible for rendering the root shop page.
    /// </summary>
    [PluginController("Bazaar")]
    public partial class BazaarStoreController : BazaarControllerBase
    {
        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public override ActionResult Index(RenderModel model)
        {
            var viewModel = ViewModelFactory.CreateStore(model);

            return this.View(viewModel.ThemeViewPath("Store"), viewModel);
        }

        #region Child Actions

        /// <summary>
        /// Renders recently viewed items.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public ActionResult RecentlyViewed()
        {
            var boxes = this.CustomerContext.GetRecentlyViewedProducts();
            return this.PartialView(PathHelper.GetThemePartialViewPath(BazaarContentHelper.GetStoreTheme(), "RecentlyViewed"), boxes);
        }

        #endregion
    }
}