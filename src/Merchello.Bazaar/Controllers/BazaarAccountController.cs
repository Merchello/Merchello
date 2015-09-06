namespace Merchello.Bazaar.Controllers
{
    using System;
    using System.Web.Mvc;

    using Umbraco.Core.Logging;
    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar account controller.
    /// </summary>
    [PluginController("Bazaar")]
    [Authorize]
    public class BazaarAccountController : CheckoutRenderControllerBase
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
            // add in webconfig appsetting    <add key="umbracoHomeStoreUrl" value="/store" />
            string homeUrl = Convert.ToString(ConfigurationManager.AppSettings["umbracoHomeStoreUrl"]);
            if (CurrentCustomer.IsAnonymous)
            {
                var error = new Exception("Current customer cannot be Anonymous");
                LogHelper.Error<BazaarAccountController>("Anonymous customers should not be allowed to access the Account section.", error);
                  //throw error;
                return this.Redirect(homeUrl);
            }

            var viewModel = ViewModelFactory.CreateAccount(model, AllCountries, AllowedShipCountries);

            return this.View(viewModel.ThemeAccountPath("Account"), viewModel);
        }

    }
}
