namespace Merchello.FastTrack.Controllers.Membership
{
    using System;
    using System.Web.Mvc;
    using System.Web.Security;

    using Merchello.Core.Logging;
    using Merchello.FastTrack.Factories;
    using Merchello.FastTrack.Models.Membership;
    using Merchello.Web.Controllers;

    using Umbraco.Core;
    using Umbraco.Core.Services;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.Security;

    /// <summary>
    /// A controller responsible for rendering and handling membership operations.
    /// </summary>
    [PluginController("FastTrack")]
    public class CustomerMembershipController : MerchelloUIControllerBase
    {
        /// <summary>
        /// The <see cref="IMemberService"/>.
        /// </summary>
        private readonly IMemberService _memberService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerMembershipController"/> class.
        /// </summary>
        public CustomerMembershipController()
            : this(new NewMemberModelFactory<NewMemberModel>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerMembershipController"/> class.
        /// </summary>
        /// <param name="newMemberModelFactory">
        /// The new member model factory.
        /// </param>
        public CustomerMembershipController(NewMemberModelFactory<NewMemberModel> newMemberModelFactory)
        {
            Mandate.ParameterNotNull(newMemberModelFactory, "newMemberModelFactory");
            NewMemberModelFactory = newMemberModelFactory;

            _memberService = ApplicationContext.Current.Services.MemberService;
        }

        /// <summary>
        /// Gets the <see cref="NewMemberModelFactory{TModel}"/>.
        /// </summary>
        protected NewMemberModelFactory<NewMemberModel> NewMemberModelFactory { get; private set; }

        /// <summary>
        /// Handles the membership login operation.
        /// </summary>
        /// <param name="model">
        /// The <see cref="LoginModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public virtual ActionResult Login(LoginModel model)
        {
            return Redirect("/");
        }

        /// <summary>
        /// Handles the account registration.
        /// </summary>
        /// <param name="model">
        /// The <see cref="NewMemberModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public virtual ActionResult Register(NewMemberModel model)
        {
            var logData = new ExtendedLoggerData();
            logData.AddCategory("Merchello");
            logData.AddCategory("FastTrack");

            var registerModel = Members.CreateRegistrationModel(model.MemberTypeAlias);
            registerModel.Name = model.Email;
            registerModel.Email = model.Email;
            registerModel.Password = model.Password;
            registerModel.Username = model.Email;
            registerModel.UsernameIsEmail = true;

            MembershipCreateStatus status;

            //// Add a message for the attempt
            MultiLogHelper.Info<CustomerMembershipController>("Registering a new member", logData);

            Members.RegisterMember(registerModel, out status, model.PersistLogin);

            var registration = NewMemberModelFactory.Create(model, status);

            if (registration.ViewData.Success)
            {
                var member = _memberService.GetByEmail(model.Email);
                if (member != null)
                {
                    if (member.HasProperty("firstName")) member.SetValue("firstName", model.FirstName);
                    if (member.HasProperty("lastName")) member.SetValue("lastName", model.LastName);

                    _memberService.Save(member);
                    _memberService.AssignRole(member.Id, "Customers");
                    _memberService.Save(member);
                }

                return model.SuccessRedirectUrl.IsNullOrWhiteSpace()
                           ? Redirect("/")
                           : Redirect(model.SuccessRedirectUrl);
            }
            else
            {
                ViewData["MerchelloViewData"] = model.ViewData;
                return CurrentUmbracoPage();
            }
        }

        /// <summary>
        /// Renders the login form.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult LoginForm(string view = "")
        {
            var model = new LoginModel { RememberMe = true };
            return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
        }

        /// <summary>
        /// Renders the registration form.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult RegisterForm(string view = "")
        {
            var model = NewMemberModelFactory.Create(CurrentCustomer);
            return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
        }
    }
}