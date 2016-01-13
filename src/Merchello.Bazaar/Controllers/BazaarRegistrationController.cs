namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar customer sign up controller.
    /// </summary>
    [PluginController("Bazaar")]
    public partial class BazaarRegistrationController : BazaarControllerBase
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
            var viewModel = ViewModelFactory.CreateRegistration(model);
          
            return this.View(viewModel.ThemeAccountPath("Registration"), viewModel);
        }
    }
}