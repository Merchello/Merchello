namespace Merchello.Bazaar.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core.Models;

    using Umbraco.Core.Logging;
    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar account controller.
    /// </summary>
    [PluginController("Bazaar")]
    public class BazaarAccountController : RenderControllerBase
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
            var viewModel = (AccountModel)this.Populate(new AccountModel(model.Content));
            if (viewModel.CurrentCustomer.IsAnonymous)
            {
                var error = new Exception("Current customer cannot be Anonymous");
                LogHelper.Error<BazaarAccountController>("Anonymous customers should not be allowed to access the Account section.", error);
                throw error;
            }

            var customer = (ICustomer)viewModel.CurrentCustomer;

            viewModel.Profile = new CustomerProfileModel()
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                EmailAddress = customer.Email
            };

            return this.View(viewModel.ThemeAccountPath("Account"), viewModel);
        }
    }
}