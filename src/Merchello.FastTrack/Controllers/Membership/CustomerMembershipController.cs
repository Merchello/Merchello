namespace Merchello.FastTrack.Controllers.Membership
{
    using System.Web.Mvc;

    using Merchello.FastTrack.Factories;
    using Merchello.FastTrack.Models.Membership;
    using Merchello.Web.Controllers;

    using Umbraco.Core;

    /// <summary>
    /// A controller responsible for rendering and handling membership operations.
    /// </summary>
    public class CustomerMembershipController : MerchelloUIControllerBase
    {
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
        }

        /// <summary>
        /// Renders the login form.
        /// </summary>
        /// <param name="view">
        /// The view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult LoginForm(string view = "")
        {
            var model = new LoginModel();
            return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
        }
    }
}