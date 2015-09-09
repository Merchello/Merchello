namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar product content controller.
    /// </summary>
    [PluginController("Bazaar")]
    public class BazaarProductContentController : BazaarControllerBase
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
            var theme = BazaarContentHelper.GetStoreTheme();
            return this.View(PathHelper.GetThemeViewPath(theme, "ProductContent"), model.Content);
        }
    }
}