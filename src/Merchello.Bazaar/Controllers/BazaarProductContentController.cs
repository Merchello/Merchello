namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar product content controller.
    /// </summary>
    [PluginController("Bazaar")]
    public partial class BazaarProductContentController : BazaarControllerBase
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

            ((IProductContent)model.Content).SpecifyCulture(UmbracoContext.PublishedContentRequest.Culture);

            return this.View(PathHelper.GetThemeViewPath(theme, "ProductContent"), model.Content);
        }
    }
}