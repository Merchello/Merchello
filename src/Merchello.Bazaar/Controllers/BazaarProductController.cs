using System;
using Merchello.Bazaar.Models;
using Merchello.Web.Models.VirtualContent;

namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// Controller responsible for displaying Merchello Product.
    /// </summary>
    [Obsolete("This controller is no longer used, see BazaarProductContentController")]
    [PluginController("Bazaar")]
    public partial class BazaarProductController : BazaarControllerBase
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
            // Create the viewmodel
            var viewModel = ViewModelFactory.CreateProduct(model);

            return this.View(viewModel.ThemeViewPath("Product"), viewModel);
        }
    }
}