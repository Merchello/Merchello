namespace Merchello.Bazaar.Controllers
{
    using System;
    using System.Data;
    using System.Web.Mvc;
    using System.Web.Security;

    using Merchello.Bazaar.Models;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The membership controller.
    /// </summary>
    [PluginController("Bazaar")]
    public class MembershipController : SurfaceControllerBase
    {
        /// <summary>
        /// Responsible for rendering the member registration form.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="theme">
        /// The current theme
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public ActionResult RenderSignUpForm(CustomerRegistrationModel model, string theme)
        {
            return View(PathHelper.GetThemePartialViewPath(theme, "SignInSignUp"));
        }

        /// <summary>
        /// Handles the membership registration.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult HandleSignUp(CombinedRegisterLoginModel model)
        {
            // Model validation
            if (!ModelState.IsValid) return this.CurrentUmbracoPage();

            var registerModel = Members.CreateRegistrationModel(model.Registration.MemberTypeName.DecryptWithMachineKey());
            registerModel.Name = model.Registration.EmailAddress;
            registerModel.Email = model.Registration.EmailAddress;
            registerModel.Password = model.Registration.Password;
            registerModel.Username = model.Registration.EmailAddress;
            registerModel.CreatePersistentLoginCookie = model.Registration.PersistLogin;
            registerModel.UsernameIsEmail = true;

            if (registerModel.MemberProperties.Exists(x => x.Alias == "firstName"))
                registerModel.MemberProperties.Find(x => x.Alias == "firstName").Value = model.Registration.FirstName;

            if (registerModel.MemberProperties.Exists(x => x.Alias == "lastName"))
                registerModel.MemberProperties.Find(x => x.Alias == "lastName").Value = model.Registration.LastName;

            MembershipCreateStatus status;
            var membershipUser = Members.RegisterMember(registerModel, out status);

            switch (status)
            {
                case MembershipCreateStatus.InvalidPassword:
                    ModelState.AddModelError(string.Empty, "Invalid password");
                    return this.CurrentUmbracoPage();

                case MembershipCreateStatus.DuplicateEmail:
                    ModelState.AddModelError(string.Empty, "The email address " + model.Registration.EmailAddress + " is already associated with a customer.");
                    return this.CurrentUmbracoPage();

                case MembershipCreateStatus.DuplicateUserName:
                    ModelState.AddModelError(string.Empty, "The email address " + model.Registration.EmailAddress + " is already associated with a customer.");
                    return this.CurrentUmbracoPage();

                default:
                    // TODO should not need to do this but need to ask for a better event in Umbraco
                    // but have to resave to fire off the memberservice event to populate the Merchello customer
                    // correctly
                    var member = Services.MemberService.GetByEmail(model.Registration.EmailAddress);
                    if (member != null) Services.MemberService.Save(member);
                    break;
            }

            return RedirectToUmbracoPage(model.AccountPageId);
        }

        /// <summary>
        /// The handle sign in.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult HandleSignIn(CombinedRegisterLoginModel model)
        {
            if (Members.Login(model.Login.Username, model.Login.Password))
            {
                // successful login
                return this.RedirectToUmbracoPage(model.AccountPageId);
            }
            else
            {
                // unsuccessful login
                return this.CurrentUmbracoPage();
            }
        }

        /// <summary>
        /// The handle sign out.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public ActionResult HandleSignOut()
        {
            Members.Logout();
            return this.Redirect("/");
        }
    }
}