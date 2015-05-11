namespace Merchello.Plugin.Payments.Braintree.Bazaar.Controllers
{
    using Merchello.Web.Mvc;

    /// <summary>
    /// The braintree transaction controller base.
    /// </summary>
    public abstract class BraintreeTransactionControllerBase : PaymentMethodCheckoutController
    {
        /// <summary>
        /// The view path.
        /// </summary>
        private const string ViewPath = "~/App_Plugins/Merchello.Braintree/Views/Partials/";

        /// <summary>
        /// Helper method to construct the path to the MVC Partial view for this plugin.
        /// </summary>
        /// <param name="viewName">
        /// The view name.
        /// </param>
        /// <returns>
        /// The virtual path to the partial view.
        /// </returns>
        protected string BraintreePartial(string viewName)
        {
            return string.Format("{0}/{1}", ViewPath, viewName);
        }
    }
}