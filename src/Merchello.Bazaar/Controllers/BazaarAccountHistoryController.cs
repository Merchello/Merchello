namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Merchello.Bazaar.Models.ViewModels;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar account history controller.
    /// </summary>
    [PluginController("Bazaar")]
    [Authorize]
    public class BazaarAccountHistoryController : RenderControllerBase
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
            var viewModel = this.Populate(new AccountHistoryModel(model.Content));

            return this.View(viewModel.ThemeAccountPath("History"), viewModel);
        }
    }
}