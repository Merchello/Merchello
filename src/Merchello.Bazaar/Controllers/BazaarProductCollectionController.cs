namespace Merchello.Bazaar.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Umbraco.Core.Persistence.DatabaseModelDefinitions;
    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar product collection controller.
    /// </summary>
    [PluginController("Bazaar")]
    public class BazaarProductCollectionController : BazaarControllerBase
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
            var viewModel = ViewModelFactory.CreateProductCollection(model);

            viewModel.SpecifyCulture(UmbracoContext.PublishedContentRequest.Culture);            

            return this.View(viewModel.ThemeViewPath("ProductCollection"), viewModel);
        }
    }
}