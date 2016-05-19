namespace Merchello.Web.Store.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core.Models;
    using Merchello.Web.Controllers;
    using Merchello.Web.Models.Ui.Rendering;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The customer membership controller.
    /// </summary>
    public abstract class CustomerMembershipControllerBase : MerchelloUIControllerBase
    {
        /// <summary>
        /// Responsible for rendering the customer sales history.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        [Authorize]
        public virtual ActionResult CustomerSalesHistory(string view = "")
        {
            var customer = (ICustomer)CurrentCustomer;
            var model = new CustomerSalesHistory(customer);

            return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
        } 
    }
}