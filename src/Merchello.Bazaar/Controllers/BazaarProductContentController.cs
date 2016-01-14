using Merchello.Bazaar.Models;

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

            // Get the product as IProductContent
            var product = model.Content as IProductContent;

            // Specify Culture
            product.SpecifyCulture(UmbracoContext.PublishedContentRequest.Culture);

            // Add a recently viewed key
            this.CustomerContext.AddRecentKey(product);

            return this.View(PathHelper.GetThemeViewPath(theme, "ProductContent"), model.Content);
        }
    }
}