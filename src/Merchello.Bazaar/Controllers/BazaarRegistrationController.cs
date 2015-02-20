namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Bazaar.Models.ViewModels;

    using Umbraco.Core;
    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar customer sign up controller.
    /// </summary>
    [PluginController("Bazaar")]
    public class BazaarRegistrationController : RenderControllerBase
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
            var viewModel = (RegistrationModel)this.Populate(new RegistrationModel(model.Content));
            viewModel.RegistrationLogin = new CombinedRegisterLoginModel()
                {
                    Login = new CustomerLoginModel(),
                    Registration = new CustomerRegistrationModel
                            {
                                MemberTypeName = viewModel.CustomerMemberTypeName.EncryptWithMachineKey(),
                            },
                    AccountPageId = viewModel.AccountPage.Id
                }; 
                
            return this.View(viewModel.ThemeAccountPath("Registration"), viewModel);
        }
    }
}