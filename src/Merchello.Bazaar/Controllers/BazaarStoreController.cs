namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// Surface controller responsible for rendering the root shop page.
    /// </summary>
    [PluginController("Bazaar")]
    public class BazaarStoreController : BazaarControllerBase
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
    }
}