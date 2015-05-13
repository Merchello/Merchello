namespace Merchello.Web.Mvc
{
    using System.Web.Mvc;

    /// <summary>
    /// An Umbraco Surface controller (abstract) that can be resolved by packages such as
    /// the Merchello Bazaar to perform various checkout operations - such as capture payments
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of the model passed to the controller
    /// </typeparam>
    public abstract class PaymentMethodUiController<TModel> : MerchelloSurfaceController, IPaymentMethodUiController
        where TModel : class
    {
        /// <summary>
        /// Responsible for rendering the payment method for a payment method in a store.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/> - Partial View.
        /// </returns>
        [ChildActionOnly]
        public abstract ActionResult RenderForm(TModel model);
    }
}